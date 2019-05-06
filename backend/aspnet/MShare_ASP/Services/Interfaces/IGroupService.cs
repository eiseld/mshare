using MShare_ASP.API.Request;
using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services{

    /// <summary>
    /// Group related services
    /// </summary>
    public interface IGroupService{

        /// <summary>
        /// Converts DaoGroup to GroupData
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="daoGroup"></param>
        API.Response.GroupData ToGroupData(long userId, DaoGroup daoGroup);

        /// <summary>
        /// Converts list of DaoGroup to list of GroupData
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="daoGroups"></param>
        IList<API.Response.GroupData> ToGroupData(long userId, IList<DaoGroup> daoGroups);

        /// <summary>
        /// Converts DaoGroup to GroupInfo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="daoGroup"></param>
        API.Response.GroupInfo ToGroupInfo(long userId, DaoGroup daoGroup);

        /// <summary>
        /// Converts list of DaoGroup to list of GroupInfo
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="daoGroups"></param>
        IList<API.Response.GroupInfo> ToGroupInfo(long userId, IList<DaoGroup> daoGroups);

        /// <summary>
        /// Gets all groups
        /// </summary>
        Task<IList<DaoGroup>> GetGroups();

        /// <summary>
        /// Gets all groups of given user
        /// </summary>
        /// <param name="userId"></param>
        Task<IList<DaoGroup>> GetGroupsOfUser(long userId);

        /// <summary>
        /// Gets group with the given Id of given user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        Task<DaoGroup> GetGroupOfUser(long userId, long groupId);

        /// <summary>
        /// Removes a member from a group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <param name="memberId"></param>
        Task RemoveMember(long userId, long groupId, long memberId);

        /// <summary>
        /// Creates a new group
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newGroup"></param>
        Task CreateGroup(long userId, NewGroup newGroup);

    }
}