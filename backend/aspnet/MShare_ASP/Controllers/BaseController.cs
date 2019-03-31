using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MShare_ASP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers {
    /// <summary>
    /// Every custom controller should inherit from this BaseController
    /// NOTE! Do not confuse this with `ControllerBase`
    /// </summary>
    public class BaseController : ControllerBase {
        /// <summary>
        /// Common Service usable for each controller that inherits from us
        /// </summary>
        protected internal IMshareService Service { get; }

        /// <summary>
        /// Initializes a BaseController
        /// </summary>
        /// <param name="mshareService"></param>
        public BaseController(IMshareService mshareService) {
            Service = mshareService;
        }

        /// <summary>
        /// Current user's id
        /// </summary>
        /// <returns>The JTW authenticated user's ID</returns>
        /// <exception cref="InvalidOperationException">When called without an [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] tagged context</exception>
        internal long GetCurrentUserID() {
            try {
                var identity = (ClaimsIdentity)User.Identity;
                return long.Parse(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            } catch {
                throw new InvalidOperationException("Must tag controller or route with [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]!");
            }
        }
    }
}
