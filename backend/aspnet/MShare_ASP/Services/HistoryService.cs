using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class HistoryService : IHistoryService
    {
        private MshareDbContext Context { get; }
        public HistoryService(MshareDbContext context)
        {
            Context = context;
        }
        public async Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId)
        {
            var daoGroup = await Context.Groups
                                .Include(x => x.Members).ThenInclude(x => x.User)
                                .SingleOrDefaultAsync(x => x.Id == groupId);

            if (daoGroup == null)
                throw new ResourceNotFoundException("group");

            if (!daoGroup.Members.Any(y => y.UserId == userId))
                throw new ResourceForbiddenException("not_group_member");

            return await Context.History
                .Where(
                    history =>
                    history.GroupId.HasValue && groupId == history.GroupId.Value)
                .ToListAsync();
        }
        public async Task LogAddMember(long userId, long groupId, long memberId)
        {
            await LogHistory(userId, groupId, new long[] { memberId }, DaoLogType.Type.ADD, DaoLogSubType.Type.MEMBER, null);
        }
        public async Task LogRemoveMember(long userId, long groupId, long memberId, HashSet<long> affectedUsers, DaoSettlement[] participatedSettlements, DaoSpending[] mySpendings, DaoDebtor[] myDebts)
        {
            dynamic historyEntry = new ExpandoObject();

            // Removed settlements:
            if (participatedSettlements.Any())
                historyEntry.RemovedSettlements = participatedSettlements.Select(x => new
                {
                    From = x.From,
                    To = x.To,
                    Amount = x.Amount
                });

            if (mySpendings.Any())
                historyEntry.RemovedSpendings = mySpendings.Select(x => new
                {
                    Name = x.Name,
                    MoneyOwed = x.MoneyOwed,
                    Date = x.Date,
                    Debtors = x.Debtors.Select(d => new
                    {
                        DebtorId = d.DebtorUserId,
                        Debt = d.Debt
                    })
                });

            if (myDebts.Any())
                historyEntry.RemovedDebts = myDebts.Select(x => new
                {
                    DebtorId = x.DebtorUserId,
                    Debt = x.Debt,
                    SpendingRemovedBecauseOfThis = x.Spending.MoneyOwed == 0 ? (new
                    {
                        Name = x.Spending.Name,
                        CreditorId = x.Spending.CreditorUserId,
                        MoneyOwed = x.Debt // MoneyOwed = 0, because we removed this member's debt, so moneyowed 'was' the whole debt of this member
                    }) : null
                });

            await LogHistory(userId, groupId, affectedUsers.ToArray(), DaoLogType.Type.REMOVE, DaoLogSubType.Type.MEMBER, historyEntry);
        }

        public async Task LogCreateGroup(long userId, DaoGroup newGroup)
        {
            dynamic historyEntry = new ExpandoObject();

            // Name
            historyEntry.Name = newGroup.Name;

            // Id
            historyEntry.Id = newGroup.Id;

            // Log
            await LogHistory(userId, newGroup.Id, new long[] { userId }, DaoLogType.Type.CREATE, DaoLogSubType.Type.GROUP, historyEntry);
        }
        public async Task LogSettlement(long userId, DaoSettlement daoSettlement)
        {
            dynamic historyEntry = new ExpandoObject();

            // Money settled
            historyEntry.Money = daoSettlement.Amount;

            // Direction of settlement:
            historyEntry.From = daoSettlement.From;
            historyEntry.To = daoSettlement.To;

            // Log
            await LogHistory(userId, daoSettlement.GroupId, new long[] { daoSettlement.From, daoSettlement.To }, DaoLogType.Type.CREATE, DaoLogSubType.Type.SETTLEMENT, historyEntry);
        }
        public async Task LogNewSpending(long userId, DaoSpending newSpending)
        {
            dynamic historyEntry = new ExpandoObject();

            // SpendingId
            historyEntry.SpendingId = newSpending.Id;

            // Name
            historyEntry.Name = newSpending.Name;

            // Money
            historyEntry.Money = newSpending.MoneyOwed;

            // Date
            historyEntry.Date = newSpending.Date;

            // Debtors
            var debtors = newSpending.Debtors
                            .Select(x => new
                            {
                                id = x.DebtorUserId,
                                debt = x.Debt
                            });
            historyEntry.Debtors = debtors;

            // Log
            await LogHistory(userId, newSpending.GroupId, debtors.Select(x => x.id).ToArray(), DaoLogType.Type.CREATE, DaoLogSubType.Type.SPENDING, historyEntry);
        }
        public async Task LogSpendingUpdate(long userId, DaoSpending oldSpending, SpendingUpdate newSpending)
        {
            dynamic historyEntry = new ExpandoObject();

            // Make name delta
            if (oldSpending.Name != newSpending.Name)
            {
                historyEntry.oldName = oldSpending.Name;
                historyEntry.newName = newSpending.Name;
            }

            // Make money delta
            if (oldSpending.MoneyOwed != newSpending.MoneySpent)
            {
                historyEntry.oldMoney = oldSpending.MoneyOwed;
                historyEntry.newMoney = newSpending.MoneySpent;
            }

            //  Make date delta
            if (oldSpending.Date != newSpending.Date)
            {
                historyEntry.oldDate = oldSpending.Date;
                historyEntry.newDate = newSpending.Date;
            }

            // Record removed debts
            var removedDebts = oldSpending.Debtors
                                .Select(x => x.DebtorUserId)
                                .Except(newSpending.Debtors.Select(x => x.DebtorId))
                                .Join(oldSpending.Debtors.Select(x => (x.Debt, x.DebtorUserId)),
                                    (oldD) => oldD,
                                    (newD) => newD.DebtorUserId,
                                    (id, olddebt) => new
                                    {
                                        id = id,
                                        debt = olddebt.Debt
                                    });
            if (removedDebts.Any())
                historyEntry.removedDebts = removedDebts;

            // Record added debts
            var addedDebts = newSpending.Debtors
                                .Select(x => x.DebtorId)
                                .Except(oldSpending.Debtors.Select(x => x.DebtorUserId))
                                .Join(newSpending.Debtors.Select(x => (x.Debt, x.DebtorId)),
                                    (oldD) => oldD,
                                    (newD) => newD.DebtorId,
                                    (id, olddebt) => new
                                    {
                                        id = id,
                                        debt = olddebt.Debt
                                    });
            if (addedDebts.Any())
                historyEntry.addedDebts = addedDebts;

            // Record updated debts
            var updatedDebts = newSpending.Debtors
                                .Select(x => (x.Debt, x.DebtorId))
                                .Join(oldSpending.Debtors.Select(x => (x.Debt, x.DebtorUserId)),
                                (oldD) => oldD.DebtorId,
                                (newD) => newD.DebtorUserId,
                                (newdebt, olddebt) => new
                                {
                                    id = olddebt.DebtorUserId, // should be same as newdebt.DebtorUserId!
                                    oldDebt = olddebt.Debt,
                                    newDebt = newdebt.Debt
                                })
                                .Where(x => x.oldDebt != x.newDebt);
            if (updatedDebts.Any())
                historyEntry.updatedDebts = updatedDebts;

            // Log 
            await LogHistory(userId, oldSpending.GroupId, oldSpending.Debtors.Select(x => x.DebtorUserId).Union(newSpending.Debtors.Select(x => x.DebtorId)).ToArray(), DaoLogType.Type.UPDATE, DaoLogSubType.Type.SPENDING, historyEntry);
        }

		public async Task LogRemoveSpending(long userId, long groupId, DaoSpending deletedSpending, HashSet<long> affectedUsers)
		{
			dynamic historyEntry = new ExpandoObject();

			// Removed spending
			historyEntry.DeletedSpending = new
			{
				Name = deletedSpending.Name,
				MoneyOwed = deletedSpending.MoneyOwed,
                Date = deletedSpending.Date,
                Debtors = deletedSpending.Debtors.Select(d => new
				{
					DebtorId = d.DebtorUserId,
					Debt = d.Debt
				})
			};

			await LogHistory(userId, groupId, affectedUsers.ToArray(), DaoLogType.Type.REMOVE, DaoLogSubType.Type.MEMBER, historyEntry);
		}

		public async Task LogHistory(long userId, long? groupId, long[] affectedIds, DaoLogType.Type type, DaoLogSubType.Type subType, object data)
        {
            var serializedMessage = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings()
            {
                MaxDepth = 4
            });

            var historyEntity = new DaoHistory()
            {
                UserId = userId,
                AffectedIds = affectedIds,
                Date = DateTime.UtcNow,
                Type = type,
                SubType = subType,
                SerializedLog = serializedMessage,
                GroupId = groupId
            };

            await Context.AddAsync(historyEntity);
        }
    }
}
