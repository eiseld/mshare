using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmailTemplates.Controllers
{

    /// <summary>Landing controller for android app</summary>
    [Route("androidlanding")]
    public class AndroidLandingController : Controller
    {
        /// <summary>Android Landing page for changing password</summary>
        [Route("forgotpassword/{token}")]
        [HttpGet]
        public IActionResult ChangePassword(string token)
        {
            return Redirect($"elte.mshare://forgotpassword/{token}");
        }

        /// <summary>Android Landing page for validating registration</summary>
        [Route("confirmregistration/{token}")]
        [HttpGet]
        public IActionResult ValidateRegistration(string token)
        {
            return Redirect($"elte.mshare://confirmregistration/{token}");
        }
    }
}