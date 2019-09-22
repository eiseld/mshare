using FluentValidation;
using System;

namespace MShare_ASP.API.Request
{

    /// <summary>Identifies the user</summary>
    public class LoginCredentials
    {

        /// <summary>Email that the user registered with</summary>
        public String Email { get; set; }

        /// <summary>Unhashed password</summary>
        public String Password { get; set; }
    }

    /// <summary>Validator object for LoginCredentials data class</summary>
    public class LoginCredentialsValidator : AbstractValidator<LoginCredentials>
    {

        /// <summary>Initializes the validator object</summary>
        public LoginCredentialsValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
            RuleFor(x => x.Password)
                .MinimumLength(6)
                .Matches("");
        }
    }
}
