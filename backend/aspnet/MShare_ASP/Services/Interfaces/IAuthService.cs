using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace MShare_ASP.Services{

    /// <summary>
    /// Authentication related services
    /// </summary>
    public interface IAuthService {

        /// <summary>
        /// Checks the credentials of the user and signs him in.
        /// </summary>
        /// <param name="credentials">Credentials to check</param>
        /// <returns>Valid JWT or null</returns>
        string Login(API.Request.LoginCredentials credentials);

        /// <summary>
        /// Registers a new user to the database, checks for duplication
        /// </summary>
        /// <param name="newUser">The new user data to register</param>
        /// <returns>true if registration successful</returns>
        Task<bool> Register(API.Request.NewUser newUser);

        /// <summary>
        /// Validates an email token for registration and updates user state
        /// </summary>
        /// <param name="token">Token to validate</param>
        /// <returns>true if validation successful</returns>
        Task<bool> Validate(String token);

    }
}
