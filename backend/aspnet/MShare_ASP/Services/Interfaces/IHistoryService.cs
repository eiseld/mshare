using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    /// <summary>
    /// Logging service for the app, allows logging every action and retrieving logs to specific groups of users
    /// </summary>
    public interface IHistoryService
    {
        /// <summary>Gets a group's history</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
		Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId);

        /// <summary>Logs a whole or partial update of a spending</summary>
        Task LogSpendingUpdate(long userId, DaoSpending currentSpending, SpendingUpdate spendingUpdate);
        /// <summary>Logs the addition of a new spending, should be called after a savechanges in transaction, because it needs the ID of the added spending</summary>
        Task LogNewSpending(long userId, DaoSpending spending);
        /// <summary>Logs a settlement between two users</summary>
        Task LogSettlement(long userId, DaoSettlement settlement);
        /// <summary>Logs a member addition to a group</summary>
        Task LogAddMember(long userId, long groupId, long memberId);
        /// <summary>Logs the removal of a member from a group and every modified entity that comes with it</summary>
        Task LogRemoveMember(long userId, long groupId, long memberId, HashSet<long> affectedUsers, DaoSettlement[] participatedSettlements, DaoSpending[] mySpendings, DaoDebtor[] myDebts);
		/// <summary>Logs the removal of a spending from a group</summary>
		Task LogRemoveSpending(long userId, long groupId, DaoSpending deletedSpending, HashSet<long> affectedUsers);
		/// <summary>Logs a new group creation, should be called after a savechanges in transaction, becase it needs the ID of the added group</summary>
		Task LogCreateGroup(long userId, DaoGroup daoGroup);
    }
}
