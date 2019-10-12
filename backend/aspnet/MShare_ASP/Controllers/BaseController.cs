using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace MShare_ASP.Controllers
{
    /// <summary>
    /// Every custom controller should inherit from this BaseController
    /// NOTE! Do not confuse this with `ControllerBase`
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>Initializes a BaseController</summary>
        public BaseController() { }

        /// <summary>Current user's id</summary>
        /// <returns>The JTW authenticated user's ID</returns>
        /// <exception cref="InvalidOperationException">When called without an [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] tagged context</exception>
        internal long GetCurrentUserID()
        {
            try
            {
                var identity = (ClaimsIdentity)User.Identity;
                return long.Parse(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            } catch
            {
#if DEBUG
                throw new InvalidOperationException("You must tag your controller or route with [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]!");
#else
                throw new InvalidOperationException("Authentication required");
#endif
            }
        }
    }
}