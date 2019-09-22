using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using MShare_ASP.Utils;

namespace MShare_ASP.Services
{
    internal class SpendingService : ISpendingService
    {
        private MshareDbContext Context { get; }
        private IUserService UserService { get; }
        private IGroupService GroupService { get; }

        /// <summary>Runs optimization algotithm for debt</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        private async Task OptimizeSpendingForGroup(long userId, long groupId)
        {
            var daoGroup = await GroupService.GetGroupOfUser(userId, groupId);

            var daoSpendings = await GetSpendingsForGroup(userId, groupId);

            var currentGroupSettleMents = Context.Settlements
                .Where(x => x.GroupId == groupId);

            Dictionary<int, long> NumberToId = new Dictionary<int, long>();
            Dictionary<long, int> IdToNumber = new Dictionary<long, int>();
            {
                int i = 0;
                foreach (var MemberId in daoGroup.Members.Select(x => x.UserId))
                {
                    NumberToId.Add(i, MemberId);
                    IdToNumber.Add(MemberId, i);
                    i++;
                }
            }

            int ingroup = NumberToId.Count;
            long[,] owes = new long[ingroup, ingroup];
            for (int i = 0; i < ingroup; i++)
            {
                var iSpending = daoSpendings.Where(x => x.CreditorUserId == NumberToId[i]);
                foreach (DaoSpending ds in iSpending)
                {
                    foreach (DaoDebtor dd in ds.Debtors)
                    {
                        owes[IdToNumber[dd.DebtorUserId], i] += dd.Debt ?? 0;
                    }
                }
            }

            foreach (var settlement in currentGroupSettleMents)
            {
                if (daoGroup.Members.Any(x => x.UserId == settlement.From) && daoGroup.Members.Any(x => x.UserId == settlement.To))
                {
                    owes[IdToNumber[settlement.To], IdToNumber[settlement.From]] += settlement.Amount;
                }
            }

            for (int i = 0; i < ingroup; i++)
            {
                owes[i, i] = 0;
            }

            var Optimizer = new SpendingOptimizer(owes, ingroup);
            Optimizer.Optimize();
            owes = Optimizer.GetResult();
            var oldOptimized = await Context.OptimizedDebt.Where(x => x.GroupId == groupId).ToListAsync();
            Context.OptimizedDebt.RemoveRange(oldOptimized);
            for (int i = 0; i < ingroup; i++)
            {
                for (int j = 0; j < ingroup; j++)
                {
                    if (owes[i, j] > 0)
                    {
                        DaoOptimizedDebt optdebt = new DaoOptimizedDebt()
                        {
                            GroupId = groupId,
                            UserOwesId = NumberToId[i],
                            UserOwedId = NumberToId[j],
                            OweAmount = owes[i, j]
                        };
                        await Context.OptimizedDebt.AddAsync(optdebt);
                    }
                }
            }
            await Context.SaveChangesAsync();
            //save results
        }


        public SpendingService(MshareDbContext context, IUserService userService, IGroupService groupService)
        {
            Context = context;
            UserService = userService;
            GroupService = groupService;
        }

        public SpendingData ToSpendingData(DaoSpending daoSpending)
        {
            return new SpendingData()
            {
                Name = daoSpending.Name,
                Creditor = new UserData()
                {
                    Id = daoSpending.Creditor.Id,
                    Name = daoSpending.Creditor.DisplayName
                },
                CreditorUserId = daoSpending.CreditorUserId,
                Id = daoSpending.Id,
                MoneyOwed = daoSpending.MoneyOwed,
                Debtors = daoSpending.Debtors.Select(daoDebtor => new DebtorData()
                {
                    Id = daoDebtor.DebtorUserId,
                    Name = daoDebtor.Debtor.DisplayName,
                    Debt = daoDebtor.Debt.Value
                }).ToList()
            };
        }

        public IList<SpendingData> ToSpendingData(IList<DaoSpending> daoSpendings)
        {
            return daoSpendings.Select(daoSpending => ToSpendingData(daoSpending)).ToList();
        }

        public OptimisedDebtData ToOptimisedDebtData(DaoOptimizedDebt daoOptimizedDebt)
        {
            return new OptimisedDebtData()
            {
                Debtor = new UserData()
                {
                    Id = daoOptimizedDebt.UserOwes.Id,
                    Name = daoOptimizedDebt.UserOwes.DisplayName
                },
                Creditor = new UserData()
                {
                    Id = daoOptimizedDebt.UserOwed.Id,
                    Name = daoOptimizedDebt.UserOwed.DisplayName
                },
                OptimisedDebtAmount = daoOptimizedDebt.OweAmount
            };
        }

        public IList<OptimisedDebtData> ToOptimisedDebtData(IList<DaoOptimizedDebt> optimizedDebts)
        {
            return optimizedDebts.Select(daoOptimizedDebt => ToOptimisedDebtData(daoOptimizedDebt)).ToList();
        }

        public async Task<IList<DaoSpending>> GetSpendingsForGroup(long userId, long groupId)
        {
            //Security check
            await GroupService.GetGroupOfUser(userId, groupId);

            return await Context.Spendings
                .Include(x => x.Creditor)
                .Include(x => x.Debtors).ThenInclude(x => x.Debtor)
                .Where(x => x.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<IList<DaoOptimizedDebt>> GetOptimizedDebtForGroup(long userId, long groupId)
        {
            return await Context.OptimizedDebt.Where(x => x.GroupId == groupId)
                .Include(x => x.UserOwed)
                .Include(x => x.UserOwes)
                .ToListAsync();
        }

        public async Task CreateNewSpending(long userId, NewSpending newSpending)
        {
            var daoGroup = await GroupService.GetGroupOfUser(userId, newSpending.GroupId);

            if (!newSpending.Debtors.All(x => daoGroup.Members.Any(m => m.UserId == x.DebtorId)))
                throw new BusinessException("debtor_not_member");

            DaoSpending spending = new DaoSpending()
            {
                Name = newSpending.Name,
                MoneyOwed = newSpending.MoneySpent,
                Group = daoGroup,
                GroupId = daoGroup.Id,
                Creditor = await UserService.GetUser(userId),
                CreditorUserId = userId
            };

            var debtors = newSpending.Debtors.Select(x => new DaoDebtor()
            {
                Spending = spending,
                DebtorUserId = x.DebtorId,
                Debt = x.Debt
            }).ToList();

            spending.Debtors = debtors.ToList();

            var insertCount = 1 + spending.Debtors.Count;

            await Context.Spendings.AddAsync(spending);

            if (await Context.SaveChangesAsync() != insertCount)
                throw new DatabaseException("spending_not_inserted");

            await OptimizeSpendingForGroup(userId, newSpending.GroupId);
        }

        public async Task UpdateSpending(long userId, SpendingUpdate spendingUpdate)
        {
            var daoGroup = await GroupService.GetGroupOfUser(userId, spendingUpdate.GroupId);

            if (!spendingUpdate.Debtors.All(x => daoGroup.Members.Any(m => m.UserId == x.DebtorId)))
                throw new BusinessException("debtor_not_member");

            var currentSpending = await Context.Spendings
                                               .Include(x => x.Debtors)
                                              .SingleOrDefaultAsync(x => x.Id == spendingUpdate.Id);

            if (currentSpending == null)
                throw new ResourceGoneException("spending");

            if (currentSpending.CreditorUserId != userId)
                throw new ResourceForbiddenException("not_creditor");

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    currentSpending.Name = spendingUpdate.Name;
                    currentSpending.MoneyOwed = spendingUpdate.MoneySpent;

                    Context.Debtors.RemoveRange(currentSpending.Debtors);
                    var debtors = spendingUpdate.Debtors.Select(x => new DaoDebtor()
                    {
                        Spending = currentSpending,
                        DebtorUserId = x.DebtorId,
                        Debt = x.Debt
                    }).ToList();
                    currentSpending.Debtors = debtors.ToList();

                    if (await Context.SaveChangesAsync() == 0)
                    {
                        throw new DatabaseException("spending_not_updated");
                    }

                    await OptimizeSpendingForGroup(userId, spendingUpdate.GroupId);

                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task DebtSettlement(long userId, long debtorId, long lenderId, long groupId)
        {
            if (userId != debtorId && userId != lenderId)
                throw new ResourceForbiddenException("user_not_in_transaction");

            if (!(await GroupService.GetGroupOfUser(userId, groupId))
                .Members.Any(x => (userId == lenderId && x.UserId == debtorId)
                               || (userId == debtorId && x.UserId == lenderId)))
                throw new ResourceForbiddenException("lender_or_debtor_not_member");

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var optDeb = Context.OptimizedDebt
                        .FirstOrDefault(x => x.GroupId == groupId && x.UserOwesId == debtorId && x.UserOwedId == lenderId);

                    if (optDeb == null)
                        throw new ResourceGoneException("debt");

                    if (optDeb.OweAmount == 0)
                        throw new BusinessException("debt_already_payed");

                    DaoSettlement settlement = new DaoSettlement()
                    {
                        GroupId = groupId,
                        From = debtorId,
                        To = lenderId,
                        Amount = optDeb.OweAmount
                    };

                    await Context.Settlements.AddAsync(settlement);

                    if (await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("debt_not_settled");

                    await OptimizeSpendingForGroup(userId, groupId);

                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
