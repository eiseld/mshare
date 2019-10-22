using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    public interface IHistoryService
    {
        /// <summary>Gets a group's history</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
		Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId);

        Task LogSpendingUpdate(long userId, DaoSpending currentSpending, SpendingUpdate spendingUpdate);
        Task LogNewSpending(long userId, DaoSpending spending);
        Task LogSettlement(long userId, DaoSettlement settlement);
        Task LogAddMember(long userId, long groupId, long memberId);
        Task LogRemoveMember(long userId, long groupId, long memberId);
        Task LogCreateGroup(long userId, DaoGroup daoGroup);
    }
}
