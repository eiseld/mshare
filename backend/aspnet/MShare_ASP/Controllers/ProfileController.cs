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
        public ProfileController(IMshareService mshareService) :
            base(mshareService) {
        }

        /// <summary>
        /// Gets the current signed in user
        /// </summary>
        /// <response code="200">Returns with the user's profile data</response>
        [HttpGet]
        public async Task<ActionResult<DaoUser>> Get() {
            return Ok(await Service.GetUser(GetCurrentUserID()));
        }
    }
}