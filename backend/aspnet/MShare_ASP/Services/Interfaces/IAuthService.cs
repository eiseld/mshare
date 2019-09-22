using System;
using System.Threading.Tasks;
using MShare_ASP.API.Request;
using MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Services
{

    /// <summary>Authentication related services</summary>
    public interface IAuthService
    {

        /// <summary>Checks the credentials of the user and signs him in.</summary>
        /// <exception cref="BusinessException">["not_verified"]</exception>
        /// <exception cref="ResourceForbiddenException">["invalid_credentials"]</exception>
        /// <returns>Valid JWT or null</returns>
        string Login(LoginCredentials credentials);

        /// <summary>Registers a new user to the database</summary>
        /// <exception cref="BusinessException">["email_taken"]</exception>
        /// <exception cref="DatabaseException">["registration_not_saved"]</exception>
        Task Register(NewUser newUser);

        /// <summary>Validates an email token for registration</summary>
        /// <exception cref="DatabaseException">["validation_email_remove_failed"]</exception>
        /// <exception cref="ResourceGoneException">["token_invalid_or_expired"]</exception>
        Task ValidateRegistration(String token);
    }
}
