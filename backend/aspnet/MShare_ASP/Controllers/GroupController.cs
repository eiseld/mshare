using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MShare_ASP.API.Response;
using MShare_ASP.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers
{
    /// <summary>GroupController is responsible for Group related actions</summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class GroupController : BaseController
    {
        private IGroupService GroupService { get; }
        private ISpendingService SpendingService { get; }
        private IHistoryService HistoryService { get; }

        /// <summary>Initializes the GroupController</summary>
        public GroupController(IGroupService groupService, ISpendingService spendingService, IHistoryService historyService)
        {
            GroupService = groupService;
            SpendingService = spendingService;
            HistoryService = historyService;
        }

#if DEBUG

        /// <summary>Lists all groups (DEBUG ONLY)</summary>
        /// <response code="200">Successfully returned all groups</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IList<GroupData>>> Get()
        {
            //TODO This will fail if user with id 1 not in all group
            var groupData = GroupService.ToGroupData(1, await GroupService.GetGroups());
            return Ok(groupData);
        }

#endif

        /// <summary>Gets the basic information of the given group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned basic information of the group</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("{groupId}/info")]
        public async Task<ActionResult<GroupInfo>> GetGroupInfo(long groupId)
        {
            var userId = GetCurrentUserID();
            var groupInfo = await GroupService.ToGroupInfo(userId, await GroupService.GetGroupOfUser(userId, groupId));
            return Ok(groupInfo);
        }

        /// <summary>Gets the full information of the given group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned full information of the group</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("{groupId}/data")]
        public async Task<ActionResult<GroupData>> GetGroupData(long groupId)
        {
            var userId = GetCurrentUserID();
            var groupData = await GroupService.ToGroupData(userId, await GroupService.GetGroupOfUser(userId, groupId));
            return Ok(groupData);
        }

        /// <summary>Adds a member to a group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <param name="memberId">Id of the new member</param>
        /// <response code="200">Successfully added member to group</response>
        /// <response code="403">Forbidden: 'not_group_member', 'not_group_creator'</response>
        /// <response code="404">Not found: 'group'</response>
        /// <response code="409">Conflict: 'user_already_member'</response>
        /// <response code="500">Internal error: 'group_member_not_added'</response>
        [HttpPost]
        [Route("{groupId}/members/add/{memberId}")]
        public async Task<ActionResult> AddMember(long groupId, long memberId)
        {
            await GroupService.AddMember(GetCurrentUserID(), groupId, memberId);
            return Ok();
        }

        /// <summary>Removes a member from a group</summary>
        /// <param name="groupId">Id of the group</param>
        /// <param name="memberId">Id of the member to be removed</param>
        /// <response code="200">Successfully added member to group</response>
        /// <response code="403">Forbidden: 'not_group_member', 'not_group_creator'</response>
        /// <response code="404">Not found: 'group', 'member'</response>
        /// <response code="409">Conflict: 'remove_creator'</response>
        /// <response code="500">Internal error: 'group_member_not_removed'</response>
        [HttpPost]
        [Route("{groupId}/members/remove/{memberId}")]
        public async Task<ActionResult> RemoveMember(long groupId, long memberId)
        {
            await GroupService.RemoveMember(GetCurrentUserID(), groupId, memberId);
            return Ok();
        }

        /// <summary>Creates a group</summary>
        /// <param name="newGroup">The new group to be created</param>
        /// <response code="200">Successfully created group</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="409">Conflict: 'name_taken'</response>
        /// <response code="500">Internal error: 'group_not_created'</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] API.Request.NewGroup newGroup)
        {
            await GroupService.CreateGroup(GetCurrentUserID(), newGroup);
            return Ok();
        }

        /// <summary>Deletes a group</summary>
        /// <param name="groupId">GroupId to delete</param>
        /// <response code="200">Successfully deleted group</response>
        /// <response code="403">Forbidden: 'not_group_creator'</response>
        /// <response code="404">Not found: 'group'</response>
        /// <response code="500">Internal error: 'group_not_deleted'</response>
        [HttpDelete]
        [Route("delete/{groupId}")]
        public async Task<ActionResult> Delete(long groupId)
        {
            await GroupService.DeleteGroup(GetCurrentUserID(), groupId);
            return Ok();
        }

        /// <summary>Gets users that match the filter</summary>
        /// <param name="filter">The filter term</param>
        /// <response code="200">Successfully returned filtered users</response>
		[HttpGet()]
        [Route("searchinallusers/{filter}")]
        public async Task<ActionResult<IList<FilteredUserData>>> GetFilteredUsers(string filter)
        {
            var filteredUsers = await GroupService.GetFilteredUsers(filter);
            return Ok(filteredUsers);
        }

        /// <summary>Gets the group history</summary>
        /// <param name="groupId">Id of the group</param>
        /// <param name="startIndex">StartIndex of the return list (skip this many from the start)</param>
        /// <param name="count">How many elements to return (0 for all of them)</param>
        /// <response code="200">Successfully returned group history</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet()]
        [Route("{groupId}/history/{startIndex}/{count}")]
        public async Task<ActionResult<IList<Data.DaoHistory>>> GetGroupHistory(long groupId, int startIndex, int count = 0)
        {
            var groupHistory = await HistoryService.GetGroupHistory(GetCurrentUserID(), groupId);
            var ordered = groupHistory.OrderByDescending(x => x.Date).Skip(startIndex);
            if(count != 0)
            {
                ordered = ordered.Take(count).ToList();
            }

            groupHistory = ordered.ToList();
            return Ok(groupHistory);
        }

        /// <summary>Settles a debt</summary>
        /// <param name="debtorId">Id of the debtor</param>
        /// <param name="lenderId">Id of the lender</param>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully settled debt</response>
        /// <response code="403">Forbidden: 'not_group_member', 'lender_not_member'</response>
        /// <response code="404">Not found: 'group'</response>
        /// <response code="409">Conflict: 'debt_already_payed'</response>
        /// <response code="410">Gone: 'debt'</response>
        /// <response code="500">Internal error: 'debt_not_settled'</response>
        [HttpPost("{groupId}/settledebt/{debtorId}/{lenderId}")]
        public async Task<ActionResult> DebtSettlement(long debtorId, long lenderId, long groupId)
        {
            await SpendingService.DebtSettlement(GetCurrentUserID(), debtorId, lenderId, groupId);
            return Ok();
        }
    }
}