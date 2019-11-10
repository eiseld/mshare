using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System;
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

        private class DateData
        {
            public string DateString { get; }
            public bool IsFutureDate { get; }
            public bool IsValidDate { get; }

            public DateData(string dateInput)
            {
                if (dateInput == "")
                {
                    DateString = string.Format("{0:yyyy-MM-ddTHH:mm:ssZ}", DateTime.UtcNow);
                    IsFutureDate = false;
                    IsValidDate = true;
                } else
                {
                    IsValidDate = DateTime.TryParse(dateInput, out DateTime dateTime);
                    if (IsValidDate)
                    {
                        DateString = dateInput;
                        IsFutureDate = dateTime > DateTime.UtcNow;
                    }
                }
            }
        }

        private async Task UpdateFutureDates(long groupId)
        {
            var futureSpendings = await Context.Spendings
              .Where(x => x.GroupId == groupId && x.IsFutureDate)
              .ToListAsync();

            foreach (var futureSpending in futureSpendings)
            {
                var dateTime = DateTime.Parse(futureSpending.Date);
                if (dateTime.ToUniversalTime() <= DateTime.UtcNow)
                {
                    futureSpending.IsFutureDate = false;
                }
            }
            await Context.SaveChangesAsync();
        }

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
                }).ToList(),
                Date = daoSpending.Date
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

            await UpdateFutureDates(groupId);
            await OptimizedService.OptimizeForGroup(groupId);

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

            if(newSpending.Debtors.Any(x => x.DebtorId == userId))
                throw new BusinessException("self_debt");

            var dateDate = new DateData(newSpending.Date);

            if(!dateDate.IsValidDate)
                throw new BusinessException("not_valid_date");

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
                        Date = dateDate.DateString,
                        IsFutureDate = dateDate.IsFutureDate,
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
                    await OptimizedService.OptimizeForGroup(newSpending.GroupId);

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

            var dateDate = new DateData(spendingUpdate.Date);

            if (!dateDate.IsValidDate)
                throw new BusinessException("not_valid_date");

            await HistoryService.LogSpendingUpdate(userId, currentSpending, spendingUpdate);

            currentSpending.Name = spendingUpdate.Name;
            currentSpending.MoneyOwed = spendingUpdate.MoneySpent;
            currentSpending.Date = dateDate.DateString;
            currentSpending.IsFutureDate = dateDate.IsFutureDate;


            Context.Debtors.RemoveRange(currentSpending.Debtors);

            currentSpending.Debtors = spendingUpdate.Debtors.Select(x => new DaoDebtor()
			{
				Spending = currentSpending,
				DebtorUserId = x.DebtorId,
				Debt = x.Debt
			}).ToList();
            await Context.SaveChangesAsync();
            await OptimizedService.OptimizeForGroup(spendingUpdate.GroupId);
		}

		public async Task DeleteSpending(long userId, long spendingId, long groupId)
		{

			var currentSpending = await Context.Spendings
											   .Include(x => x.Debtors)
											  .SingleOrDefaultAsync(x => x.Id == spendingId);

			if (currentSpending == null)
				throw new ResourceGoneException("spending");

			if (currentSpending.CreditorUserId != userId)
				throw new ResourceForbiddenException("not_creditor");

			//var affectedUsers = new HashSet<long>() { userId };

			var affectedUsers = new HashSet<long>() { currentSpending.CreditorUserId };

			foreach(DaoDebtor debtor in currentSpending.Debtors)
			{
				affectedUsers.Add(debtor.DebtorUserId);
			}

			using (var transaction = Context.Database.BeginTransaction())
			{
				try
				{

					Context.Spendings.Remove(currentSpending);

					await HistoryService.LogRemoveSpending(userId, groupId, currentSpending, affectedUsers);

					if (await Context.SaveChangesAsync() == 0)
					{
						throw new DatabaseException("spending_not_updated");
					}

					await OptimizedService.OptimizeForGroup(groupId);

					transaction.Commit();
				}
				catch
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
            await OptimizedService.OptimizeForGroup(groupId);
            await HistoryService.LogSettlement(userId, settlement);

            await Context.SaveChangesAsync();
        }
    }
}