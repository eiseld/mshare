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
    public class ProfileController : BaseController {

        /// <summary>
        /// Initializes the ProfileController 
        /// </summary>
        /// <param name="mshareService"></param>
        public ProfileController( IMshareService mshareService) :
            base(mshareService) {
        }

        [HttpPost]
        [Route("password/forgot")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody] API.Request.ValidEmail email /*todo: get from body email: " ... "  format, link emailbe: /reset?token=*/) {
            await Service.SendForgotPasswordMail(email);
            return Ok();
        }

        [HttpPost]
        [Route("password/update")]
        [AllowAnonymous]
        public async Task<ActionResult> UpdatePassword([FromBody] API.Request.PasswordUpdate passwordUpdate /*todo: get from body email: " ... ", token: " ... ", password: " ... "  format*/) {
            await Service.UpdatePassword(passwordUpdate);
            return Ok();
        }

        /// <summary>
        /// Gets the current signed in user
        /// </summary>
        /// <response code="200">Returns with the user's profile data</response>
        [HttpGet]
        public async Task<ActionResult<API.Response.UserData>> Get() {
            var a = await Service.GetUser(GetCurrentUserID());
            var b = new API.Response.UserData() {
                DisplayName = a.DisplayName,
                Groups = a.Groups.Select(x => new API.Response.GroupData() {
                    Id = x.Group.Id,
                    CreatorUser = new API.Response.UserData() {
                        DisplayName = x.User.DisplayName,
                        Groups = null
                    },
                    Members = x.Group.Members.Select(y => new API.Response.UserData() {
                        DisplayName = y.User.DisplayName,
                        Groups = null
                    }).ToList(),
                    Name = x.Group.Name,
                    Balance = 0
                }).ToList()
            };
            return Ok(b);
        }
    }
}