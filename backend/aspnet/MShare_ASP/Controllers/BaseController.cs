using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MShare_ASP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers {
    public class BaseController : ControllerBase {
        protected IMshareService Service { get; }

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
