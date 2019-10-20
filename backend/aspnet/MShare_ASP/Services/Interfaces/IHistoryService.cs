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

        Task LogHistory(long userId, long affectedId, DaoLogType.Type type, DaoLogSubType.Type subType, object data);
    }
}
