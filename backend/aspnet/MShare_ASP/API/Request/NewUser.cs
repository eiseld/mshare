using FluentValidation;
using System;
using System.Linq;

namespace MShare_ASP.API.Request
{

    /// <summary>Represents a new user to be registered</summary>
    public class NewUser
    {

        /// <summary>Email of the user as in SMTP standard</summary>
        public String Email { get; set; }

        /// <summary>Name to be displayed</summary>
        public String DisplayName { get; set; }

        /// <summary>Unhashed password</summary>
        public String Password { get; set; }
    }

    /// <summary>Validator object for NewUser data class</summary>
    public class NewUserValidator : AbstractValidator<NewUser>
    {

        /// <summary>Initializes the validator object</summary>
        public NewUserValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Password)
                .Must(x => x.Any(char.IsLower))
                .WithMessage("Must have a lower case letter!")
                .Must(x => x.Any(char.IsUpper))
                .WithMessage("Must have an upper case letter!")
                .Must(x => x.Any(char.IsDigit))
                .WithMessage("Must have at least one digit!")
                .MinimumLength(6);

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MaximumLength(32);

        }
    }
}