using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.Data;
using MShare_ASP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers {

    /// <summary>
    /// Profile controller contains information the currently logged in active user
    /// </summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProfileController : BaseController{

        private IGroupService GroupService { get; }
        private IUserService UserService { get; }

        /// <summary>
        /// Initializes the ProfileController 
        /// </summary>
        /// <param name="groupService"></param>
        /// <param name="userService"></param>
        public ProfileController(IGroupService groupService, IUserService userService){
            GroupService = groupService;
            UserService = userService;
        }


        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="email">Id of the group</param>
        /// <returns></returns>
        [HttpPost]
        [Route("password/forgot")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] API.Request.ValidEmail email){
            await UserService.SendForgotPasswordMail(email);
            return Ok();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="passwordUpdate">Id of the group</param>
        /// <returns></returns>
        [HttpPost]
        [Route("password/update")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdatePassword([FromBody] API.Request.PasswordUpdate passwordUpdate){
            await UserService.UpdatePassword(passwordUpdate);
            return Ok();
        }

        /// <summary>
        /// Gets the current signed in user
        /// </summary>
        /// <response code="200">Returns with the user's profile data</response>
        [HttpGet]
        public async Task<ActionResult<API.Response.UserData>> Get(){
            return Ok(UserService.ToUserData(await UserService.GetUser(GetCurrentUserID())));
        }

        /// <summary>
        /// Gets the basic information of user's every group
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("groups")]
        public async Task<ActionResult<IList<API.Response.GroupInfo>>> GetGroups(){
            return Ok(GroupService.ToGroupInfo(GetCurrentUserID(), await GroupService.GetGroupsOfUser(GetCurrentUserID())));
        }

        /// <summary>
        /// Gets the member data of the user
        /// </summary>
        /// <param name="fromGroup">Id of the group</param>
        /// <response code="404">Resource not found: 'group_not_found'</response>
        [HttpGet]
        [Route("{fromGroup}")]
        public async Task<ActionResult<API.Response.MemberData>> GetMemberData(long fromGroup){
            return Ok(
                GroupService.ToGroupData(GetCurrentUserID(), await GroupService.GetGroupOfUser(GetCurrentUserID(), fromGroup))
                .Members.Single(x => x.Id == GetCurrentUserID())
                );
        }

    }
}