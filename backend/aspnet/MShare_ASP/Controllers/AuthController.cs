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
        private IUserService UserService { get; }

        /// <summary>
        /// Initializes the AuthController
        /// </summary>
        /// <param name="authService"></param>
        /// <param name="userService"></param>
        public AuthController(IAuthService authService, IUserService userService){
            AuthService = authService;
            UserService = userService;
        }

#if DEBUG
        /// <summary>
        /// Lists all users (use only for testing)
        /// </summary>
        /// <response code="200">Successfully returned all users</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpGet]
        public async Task<ActionResult<IList<API.Response.UserData>>> Get() {
            return Ok(UserService.ToUserData(await UserService.GetUsers()));
        }
#endif

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="newUser">The new user to be created</param>
        /// <response code="201">User successfully created</response>
        /// <response code="409">Conflict while registering: 'email_taken' in 'errors'</response>
        /// <response code="400">Bad request, newUser probably failed validation</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] API.Request.NewUser newUser) {
            await AuthService.Register(newUser);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Validate a new registration's email
        /// </summary>
        /// <param name="token">registration token</param>
        /// <response code="200">Validation successful</response>
        /// <response code="410">Validation failed, token already gone</response>
        /// <response code="409">Validation failed, token expired: 'token_expired' in 'errors'</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpPost]
        [Route("validate/{token}")]
        public async Task<ActionResult> Validate(String token) {
            await AuthService.Validate(token);
            return Ok();
        }

        /// <summary>
        /// Log a user in
        /// </summary>
        /// <remarks>Can be called multiple times, always returns a valid JWT!</remarks>
        /// <param name="loginCred">Credentials of the user</param>
        /// <response code="200">Successful login</response>
        /// <response code="401">Login failed because user not found</response>
        /// <response code="400">User found, but not validated email</response>
        /// <response code="409">Email has not yet been verified: 'not_verified' in 'errors'</response>
        /// <response code="500">Internal error, probably database related</response>
        [HttpPut()]
        [Route("login")]
        public ActionResult<API.Response.JWTToken> Login([FromBody] API.Request.LoginCredentials loginCred) {
            var token = AuthService.Login(loginCred);
            return Ok(new API.Response.JWTToken() {
                Token = token
            });
        }
    }
}
