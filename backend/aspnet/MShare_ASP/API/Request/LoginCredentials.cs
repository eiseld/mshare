using FluentValidation;
using MShare_ASP.Data;
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

        /// <summary> 2 character representation of the language </summary>
        public DaoLangTypes.Type Lang { get; set; }
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
            RuleFor(x => x.Lang)
                .IsInEnum()
                .WithMessage("Specified language is not supported by the server");
        }
    }
}