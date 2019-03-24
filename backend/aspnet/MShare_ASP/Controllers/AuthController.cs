using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.Services;

namespace MShare_ASP.Controllers {
    /// <summary>
    /// Authentication controller responseible for logging users in and registering new users.
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthController : BaseController {

        private IAuthService AuthService { get; }

        public AuthController(IAuthService authService, IMshareService mshareService) :
            base(mshareService) {
            AuthService = authService;
        }

#if DEBUG
        /// <summary>
        /// Lists all users (use only for testing)
        /// </summary>
        /// <response code="200">Successfully returned all users</response>
        [HttpGet]
        public ActionResult<IEnumerable<API.Response.UserData>> Get() {
            return Ok(Service.Users.Select(x => new API.Response.UserData() {
                DisplayName = x.display_name
            }));
        }
#endif

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="newUser">The new user to be created</param>
        /// <response code="201">User successfully created</response>
        /// <response code="401">Registration failed</response>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] API.Request.NewUser newUser) {
            if(await AuthService.Register(newUser))
                return StatusCode(StatusCodes.Status201Created);
            else
                return StatusCode(StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// Validate a new registration's email
        /// </summary>
        /// <param name="token">registration token</param>
        /// <response code="200">Validation successful</response>
        /// <response code="403">Validation failed</response>
        [HttpPost]
        [Route("register/{token}")]
        public async Task<ActionResult> Validate(String token) {
            if (await AuthService.Validate(token))
                return Ok();
            else
                return StatusCode(StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Log a user in
        /// </summary>
        /// <remarks>Can be called multiple times, always returns a valid JWT!</remarks>
        /// <param name="loginCred">Credentials of the user</param>
        /// <response code="200">Successful login</response>
        /// <response code="401">Login failed</response>
        [HttpPut()]
        [Route("login")]
        public ActionResult<API.Response.JWTToken> Login([FromBody] API.Request.LoginCredentials loginCred) {
            var token = AuthService.Login(loginCred);
            if (token != null)
                return Ok(new API.Response.JWTToken(){
                   Token = token
                });
            else
                return StatusCode(StatusCodes.Status401Unauthorized);
        }
    }
}
