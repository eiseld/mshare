using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class SpendingService : ISpendingService
    {
        private MshareDbContext Context { get; }
        private IUserService UserService { get; }
        private IGroupService GroupService { get; }
        private IHistoryService HistoryService { get; }
		private IOptimizedService OptimizedService { get; }

        public SpendingService(MshareDbContext context, IUserService userService, IGroupService groupService, IOptimizedService optimizedService, IHistoryService historyService)
        {
            Context = context;
            UserService = userService;
            GroupService = groupService;
            OptimizedService = optimizedService;
            HistoryService = historyService;
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
                    Debt = daoDebtor.Debt
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
            //Security check
            await GroupService.GetGroupOfUser(userId, groupId);

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

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    DaoSpending spending = new DaoSpending()
                    {
                        Name = newSpending.Name,
                        MoneyOwed = newSpending.MoneySpent,
                        CreditorUserId = userId,
                        GroupId = daoGroup.Id,
                        Creditor = await UserService.GetUser(userId),
                        Group = daoGroup
                    };

                    spending.Debtors = newSpending.Debtors.Select(x => new DaoDebtor()
                    {
                        Spending = spending,
                        DebtorUserId = x.DebtorId,
                        Debt = x.Debt
                    }).ToList();

                    await Context.Spendings.AddAsync(spending);
                    await OptimizedService.OptimizeForNewSpending(userId, newSpending);

                    await Context.SaveChangesAsync();
                    // Call log AFTER saving, so ID is present
                    await HistoryService.LogNewSpending(userId, spending);

                    await Context.SaveChangesAsync();


                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
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

            var oldDebts = currentSpending.Debtors
                .ToDictionary(debtor => debtor.DebtorUserId, debtor => debtor.Debt);

            var newDebts = spendingUpdate.Debtors
                .ToDictionary(debtor => debtor.DebtorId, debtor => debtor.Debt);

            await HistoryService.LogSpendingUpdate(userId, currentSpending, spendingUpdate);

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

            await OptimizedService.OptimizeForUpdateSpending(spendingUpdate.GroupId, userId, oldDebts, newDebts);
            await Context.SaveChangesAsync();
        }

        public async Task DebtSettlement(long userId, long debtorId, long lenderId, long groupId)
        {
            if (userId != debtorId && userId != lenderId)
                throw new ResourceForbiddenException("user_not_in_transaction");

            if (!(await GroupService.GetGroupOfUser(userId, groupId))
                .Members.Any(x => (userId == lenderId && x.UserId == debtorId)
                               || (userId == debtorId && x.UserId == lenderId)))
                throw new ResourceForbiddenException("lender_or_debtor_not_member");

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
            await OptimizedService.OptimizeForSettling(groupId, lenderId, debtorId);
            await HistoryService.LogSettlement(userId, settlement);

            await Context.SaveChangesAsync();
        }
    }
}