using FluentValidation;
using MShare_ASP.Data;
using System;

namespace MShare_ASP.API.Request
{
    /// <summary>Represents an Email</summary>
    public class ForgotPasswordRequest
    {
        /// <summary>Email as in SMTP standard</summary>
        public String Email { get; set; }

        /// <summary> 2 character representation of the language </summary>
        public DaoLangTypes.Type Lang { get; set; }
    }

    /// <summary>Validator object for Email class</summary>
    public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
    {
        /// <summary>Initializes the validator object</summary>
        public ForgotPasswordRequestValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();

            RuleFor(x => x.Lang)
                .IsInEnum()
                .WithMessage("Specified language is not supported by the server");
        }
    }
}