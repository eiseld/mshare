using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.API.Response;
using MShare_ASP.Services;

namespace MShare_ASP.Controllers
{

    /// <summary>Authentication controller responseible for logging users in and registering new users.</summary>
    [Route("[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {

        private IAuthService AuthService { get; }
        private IUserService UserService { get; }

        /// <summary>Initializes the AuthController</summary>
        public AuthController(IAuthService authService, IUserService userService)
        {
            AuthService = authService;
            UserService = userService;
        }

#if DEBUG
        /// <summary>Lists all users (DEBUG ONLY)</summary>
        /// <response code="200">Successfully returned all users</response>
        [HttpGet]
        public async Task<ActionResult<IList<UserData>>> Get()
        {
            var userData = UserService.ToUserData(await UserService.GetUsers());
            return Ok(userData);
        }
#endif

        /// <summary>Register a new user</summary>
        /// <param name="newUser">The new user to be created</param>
        /// <response code="201">User successfully created</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="409">Conflict: 'email_taken'</response>
        /// <response code="500">Internal error: 'registration_not_saved'</response>
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register([FromBody] API.Request.NewUser newUser)
        {
            await AuthService.Register(newUser);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>Validate a new registration's email</summary>
        /// <param name="token">Registration token</param>
        /// <response code="200">Validation successful</response>
        /// <response code="410">Gone: 'token_invalid_or_expired'</response>
        /// <response code="500">Internal error: 'validation_email_remove_failed'</response>
        [HttpPost]
        [Route("validate/{token}")]
        public async Task<ActionResult> Validate(String token)
        {
            await AuthService.ValidateRegistration(token);
            return Ok();
        }

        /// <summary>Log a user in</summary>
        /// <remarks>Can be called multiple times, always returns a valid JWT!</remarks>
        /// <param name="loginCred">Credentials of the user</param>
        /// <response code="200">Successful login</response>
        /// <response code="400">Possible request body validation failure</response>
        /// <response code="403">Forbidden: 'invalid_credentials'</response>
        /// <response code="409">Conflict: 'not_verified'</response>
        [HttpPut()]
        [Route("login")]
        public async Task<ActionResult<JWTToken>> Login([FromBody] API.Request.LoginCredentials loginCred)
        {
            var token = await AuthService.Login(loginCred);
            return Ok(new JWTToken()
            {
                Token = token
            });
        }
    }
}
