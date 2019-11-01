using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers
{
    /// <summary>Profile controller contains information the currently logged in active user</summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class ProfileController : BaseController
    {
        private IGroupService GroupService { get; }
        private IUserService UserService { get; }

        /// <summary>Initializes the ProfileController </summary>
        public ProfileController(IGroupService groupService, IUserService userService)
        {
            GroupService = groupService;
            UserService = userService;
        }

        /// <summary>Sends password reset email to the given email address</summary>
        /// <param name="forgotPasswordRequest">Valid email address and a language</param>
        /// <response code="200">Successfully sent password reset email (ALWAYS RETURNS THIS)</response>
        /// <response code="400">Possible request body validation failure</response>
        [HttpPost]
        [Route("password/forgot")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            await UserService.SendForgotPasswordMail(forgotPasswordRequest.Email, forgotPasswordRequest.Lang);
            return Ok();
		}

		/// <summary>Changes password of the user</summary>
		/// <param name="changePassword">Valid passwords, id and a language</param>
		/// <response code="200">Successfully changed password (ALWAYS RETURNS THIS)</response>
		/// <response code="400">Possible request body validation failure</response>
		[HttpPost]
		[Route("password/change")]
		public async Task<ActionResult> ChangePassword([FromBody] ChangePassword changePassword)
		{
			await UserService.ChangePassword(changePassword);
			return Ok();
		}

		/// <summary>Set the given password for the user with the given email address if the reset token is still valid</summary>
		/// <param name="passwordUpdate">Password update information</param>
		/// <response code="200">Successfully updated password</response>
		/// <response code="400">Possible request body validation failure</response>
		/// <response code="404">Not found: 'user'</response>
		/// <response code="410">Gone: 'token_invalid_or_expired'</response>
		/// <response code="500">Internal error: 'password_not_saved', 'token_deletion_failed'</response>
		[HttpPost]
        [Route("password/update")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdatePassword([FromBody] API.Request.PasswordUpdate passwordUpdate)
        {
            await UserService.UpdatePassword(passwordUpdate);
            return Ok();
        }

        /// <summary>Set the given bank account number for the loged in user</summary>
        /// <param name="bankAccountNumberUpdate">Bank account update information</param>
        /// <response code="200">Successfully updated bank account number</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="404">Not found: 'user'</response>
        /// <response code="500">Internal error: 'account_number_update_failed'</response>
        [HttpPost]
        [Route("bankaccountnumber/update")]
        public async Task<ActionResult> UpdateBankAccountNumber([FromBody] BankAccountNumberUpdate bankAccountNumberUpdate)
        {
            await UserService.UpdateBankAccoutNumber(GetCurrentUserID(), bankAccountNumberUpdate.BankAccountNumber);
            return Ok();
        }

        /// <summary>Gets the name and id of the signed in user</summary>
        /// <response code="200">Successfully returned user data</response>
        /// <response code="404">Not found: 'user'</response>
        [HttpGet]
        public async Task<ActionResult<UserData>> Get()
        {
            var userData = UserService.ToUserData(await UserService.GetUser(GetCurrentUserID()));
            return Ok(userData);
        }

        /// <summary>Gets the list of groups which the signed in user is a member of</summary>
        /// <response code="200">Successfully returned group infos</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("groups")]
        public async Task<ActionResult<IList<GroupInfo>>> GetGroups()
        {
            var userId = GetCurrentUserID();
            var groupInfo = GroupService.ToGroupInfo(userId, await GroupService.GetGroupsOfUser(userId));
            return Ok(groupInfo);
        }

        /// <summary>Gets the member data of the user</summary>
        /// <param name="fromGroup">Id of the group</param>
        /// <response code="200">Successfully returned member data</response>
        /// <response code="403">Forbidden: 'not_group_member'</response>
        /// <response code="404">Not found: 'group'</response>
        [HttpGet]
        [Route("{fromGroup}")]
        public async Task<ActionResult<MemberData>> GetMemberData(long fromGroup)
        {
            var userId = GetCurrentUserID();
            var groupData = await GroupService.ToGroupData(userId, await GroupService.GetGroupOfUser(userId, fromGroup));
            var memberData = groupData.Members.Single(member => member.Id == userId);
            return Ok(memberData);
        }

        /// <summary>Updates the language of the current user</summary>
        /// <param name="newLanguage">New language of the user</param>
        /// <response code="200">Successfully updated language</response>
        /// <response code="500">Update failed: 'lang_update_failed'</response>
        [HttpPut]
        [Route("lang")]
        public async Task<IActionResult> SetLang([FromBody] SetLang newLanguage)
        {
            await UserService.UpdateLang(GetCurrentUserID(), newLanguage);
            return Ok();
        }
    }
}