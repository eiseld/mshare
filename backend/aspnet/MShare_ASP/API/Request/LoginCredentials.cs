using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MShare_ASP.API.Request {
    /// <summary>
    /// Identifies the user
    /// </summary>
    public class LoginCredentials {
        /// <summary>
        /// Email that the user registered with
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Unhashed password
        /// </summary>
        public String Password { get; set; }
    }

    /// <summary>
    /// Validator object for LoginCredentials data class
    /// </summary>
    internal class LoginCredentialsValidator : AbstractValidator<LoginCredentials> {

        /// <summary>
        /// Initializese the validator object
        /// </summary>
        public LoginCredentialsValidator() {
            RuleFor(x => x.Email)
                .EmailAddress();
            RuleFor(x => x.Password)
                .MinimumLength(6)
                .Matches("");
        }
    }
}
