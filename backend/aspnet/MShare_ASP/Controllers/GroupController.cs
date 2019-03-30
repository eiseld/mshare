using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.Services;

namespace MShare_ASP.Controllers {
    /// <summary>
    /// GroupController is responsible for Group related actions
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GroupController : BaseController {

        /// <summary>
        /// Initializes the AuthController
        /// </summary>
        /// <param name="mshareService"></param>
        public GroupController(IMshareService mshareService) :
            base(mshareService) {
        }

#if DEBUG
        /// <summary>
        /// Lists all users (use only for testing)
        /// </summary>
        /// <response code="200">Successfully returned all users</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IList<API.Response.GroupData>>> Get() {
            var a = (await Service.GetGroups()).Select(x => new API.Response.GroupData() {
                Id = x.Id,
                Name = x.Name,
                CreatorUser = new API.Response.UserData() {
                    DisplayName = x.CreatorUser.DisplayName
                },
                Members = x.Members.Select(y => new API.Response.UserData() {
                    DisplayName = y.User.DisplayName
                }).ToList()
            }).ToList();
            return Ok(a);
        }
#endif

        /// <summary>
        /// Returns the group with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<API.Response.GroupData>> Get(long id) {
            var x = await Service.GetGroup(id);
            var b = new API.Response.GroupData() {
                Id = x.Id,
                Name = x.Name,
                CreatorUser = new API.Response.UserData() {
                    DisplayName = x.CreatorUser.DisplayName
                },
                Members = x.Members.Select(y => new API.Response.UserData() {
                    DisplayName = y.User.DisplayName
                }).ToList()
            };
            return Ok(b);
        }

        /// <summary>
        /// Gets the members of the given group with the given limit
        /// </summary>
        /// <param name="id">Id of the group</param>
        /// <param name="limit">Limit member count</param>
        /// <response code="404">Resource not found: 'group_not_found'</response>
        [HttpGet]
        [Route("{id}/members/{limit?}")]
        [AllowAnonymous]
        public async Task<ActionResult<IList<API.Response.UserData>>> Members(long id, int? limit = int.MaxValue) {
            var x = await Service.GetGroup(id);
            var b = x.Members.Select(y => new API.Response.UserData() {
                DisplayName = y.User.DisplayName
            }).Take(limit.Value).ToList();
            return Ok(b);
        }

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="newGroup">The new group to be created</param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] API.Request.NewGroup newGroup) {
            long forUser = GetCurrentUserID();
            await Service.CreateGroup(newGroup, forUser);
            return Ok();
        }
    }
}