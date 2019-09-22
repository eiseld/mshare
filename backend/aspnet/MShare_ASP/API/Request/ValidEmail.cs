using FluentValidation;
using System;

namespace MShare_ASP.API.Request
{

    /// <summary>Represents an Email</summary>
    public class ValidEmail
    {

        /// <summary>Email as in SMTP standard</summary>
        public String Email { get; set; }
    }

    /// <summary>Validator object for Email class</summary>
    public class EmailValidator : AbstractValidator<ValidEmail>
    {

        /// <summary>Initializese the validator object</summary>
        public EmailValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();
        }
    }
}
