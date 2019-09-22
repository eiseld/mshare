using System.Collections.Generic;
using System.Threading.Tasks;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Services
{

    /// <summary>Group related services</summary>
    public interface IGroupService
    {

        /// <summary>Converts group data from internal to facing</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        Task<GroupData> ToGroupData(long userId, DaoGroup daoGroup);

        /// <summary>Converts group datas from internal to facing</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        IList<GroupData> ToGroupData(long userId, IList<DaoGroup> daoGroups);

        /// <summary>Converts group data from internal to facing</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        Task<GroupInfo> ToGroupInfo(long userId, DaoGroup daoGroup);

        /// <summary>Converts group datas from internal to facing</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        IList<GroupInfo> ToGroupInfo(long userId, IList<DaoGroup> daoGroups);

#if DEBUG
        /// <summary>Gets all group (DEBUG ONLY)</summary>
        Task<IList<DaoGroup>> GetGroups();
#endif

        /// <summary>Gets all group of the user</summary>
        Task<IList<DaoGroup>> GetGroupsOfUser(long userId);

        /// <summary>Gets a specific group of the user</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        Task<DaoGroup> GetGroupOfUser(long userId, long groupId);

        /// <summary>Removes a member from a group</summary>
        /// <exception cref="ResourceNotFoundException">["group", "member"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member", "not_group_creator"]</exception>
        /// <exception cref="BusinessException">["remove_creator"]</exception> 
        /// <exception cref="DatabaseException">["group_member_not_removed"]</exception> 
        Task RemoveMember(long userId, long groupId, long memberId);

        /// <summary>Creates a new group</summary>
        /// <exception cref="BusinessException">["name_taken"]</exception> 
        /// <exception cref="DatabaseException">["group_not_created"]</exception> 
        Task CreateGroup(long userId, NewGroup newGroup);

        /// <summary>Match username or email with filterTerm</summary>
		Task<IList<FilteredUserData>> GetFilteredUsers(string filterTerm);

        /// <summary>Gets a group's history</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
		Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId);

        /// <summary>Adds a user to a group</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member", "not_group_creator"]</exception>
        /// <exception cref="BusinessException">["user_already_member"]</exception>
        /// <exception cref="DatabaseException">["group_member_not_added"]</exception> 
		Task AddMember(long userId, long groupId, long memberId);
    }
}