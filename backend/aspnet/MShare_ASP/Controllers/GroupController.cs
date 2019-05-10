using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MShare_ASP.Services;
using MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Controllers {

    /// <summary>
    /// GroupController is responsible for Group related actions
    /// </summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class GroupController : BaseController {

        private IGroupService GroupService { get; }

        /// <summary>
        /// Initializes the GroupController
        /// </summary>
        /// <param name="groupService"></param>
        public GroupController(IGroupService groupService){
            GroupService = groupService;
        }


#if DEBUG
        /// <summary>
        /// Lists all groups (use only for testing)
        /// </summary>
        /// <response code="200">Successfully returned all groups</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IList<API.Response.GroupData>>> Get() {
            return Ok(GroupService.ToGroupData(1,await GroupService.GetGroups()));
        }
#endif


        /// <summary>
        /// Gets the basic information of the given group
        /// </summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned basic information of the group</response>
        /// <response code="404">Resource not found: 'group_not_found'</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpGet]
        [Route("{groupId}/info")]
        public async Task<ActionResult<API.Response.GroupInfo>> GetGroupInfo(long groupId) {
            return Ok(GroupService.ToGroupInfo(GetCurrentUserID(), await GroupService.GetGroupOfUser(GetCurrentUserID(), groupId)));
        }


        /// <summary>
        /// Gets the full information of the given group
        /// </summary>
        /// <param name="groupId">Id of the group</param>
        /// <response code="200">Successfully returned full information of the group</response>
        /// <response code="404">Resource not found: 'group_not_found'</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpGet]
        [Route("{groupId}/data")]
        public async Task<ActionResult<API.Response.GroupData>> GetGroupData(long groupId){
            return Ok(GroupService.ToGroupData(GetCurrentUserID(), await GroupService.GetGroupOfUser(GetCurrentUserID(), groupId)));
        }

		/// <summary>
		/// Removes a member from a group
		/// </summary>
		/// <param name="groupId">Id of the group</param>
		/// <param name="memberId">Id of the member to be added</param>
		/// <response code="404">Resource not found: 'group_not_found'</response>
		/// <response code="403">Resource forbidden: 'not_group_creator'</response>
		/// <response code="410">Resource gone: 'member_not_found'</response>
		/// <response code="500">Internal error: 'group_not_added'</response>
		[HttpPost]
		[Route("{groupId}/members/add/{memberId}")]
		public async Task<ActionResult> AddMember(long groupId, long memberId)
		{
			await GroupService.AddMember(GetCurrentUserID(), groupId, memberId);
			return Ok();
		}


		/// <summary>
		/// Removes a member from a group
		/// </summary>
		/// <param name="groupId">Id of the group</param>
		/// <param name="memberId">Id of the member to be removed</param>
		/// <response code="404">Resource not found: 'group_not_found'</response>
		/// <response code="403">Resource forbidden: 'not_group_creator'</response>
		/// <response code="410">Resource gone: 'member_not_found'</response>
		/// <response code="500">Internal error: 'group_not_removed'</response>
		[HttpDelete]
        [Route("{groupId}/members/remove/{memberId}")]
        public async Task<ActionResult> RemoveMember(long groupId, long memberId){
            await GroupService.RemoveMember(GetCurrentUserID(), groupId, memberId);
            return Ok();
        }


        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="newGroup">The new group to be created</param>
        /// <response code="409">Business exception: 'name_taken'</response>
        /// <response code="500">Internal error: 'group_not_created'</response>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] API.Request.NewGroup newGroup) {
            await GroupService.CreateGroup(GetCurrentUserID(), newGroup);
            return Ok();
        }

		[HttpGet()]
        [Route("searchinallusers/{filter}")]
		public async Task<ActionResult<IList<API.Response.FilteredUserData>>> GetFilteredUsers(string filter)
		{
			return Ok(await GroupService.InviteUserFilter(filter));
		}

		[HttpGet()]
        [Route("{groupid}/history")]
		public async Task<ActionResult<IList<Data.DaoHistory>>> GetGroupHistory(long groupid)
		{
			return Ok(await GroupService.GetGroupHistory(groupid));
		}

		[HttpPost("{groupid}/settledebt/{debtorid}/{lenderid}")]
		public async Task<ActionResult> DebtSettlement(long debtorid, long lenderid, long groupid)
		{
            if (GetCurrentUserID() != debtorid && GetCurrentUserID() != lenderid)
                throw new ResourceForbiddenException("user_not_debtor_or_lender");
			await GroupService.DebtSettlement(debtorid, lenderid, groupid);
			return Ok();
		}
		
	}
}