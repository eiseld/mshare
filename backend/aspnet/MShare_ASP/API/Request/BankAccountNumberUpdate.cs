using FluentValidation;
using System;
using System.Linq;

namespace MShare_ASP.API.Request
{

    /// <summary>Represents a PasswordUpdate request</summary>
    public class BankAccountNumberUpdate
    {

        /// <summary>Email of the user as in SMTP standard</summary>
        public String Email { get; set; }

        /// <summary>Unhashed password</summary>
        public String BankAccountNumber { get; set; }
    }

    /// <summary>Validator object for PasswordUpdate data class</summary>
    public class BankAccountNumberValidator : AbstractValidator<BankAccountNumberUpdate>
    {

        /// <summary>Initializes the validator object</summary>
        public BankAccountNumberValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress();

			RuleFor(x => x.BankAccountNumber)
				.Must(x => x.All(char.IsDigit))
				.WithMessage("Must be digits only!")
				.Length(24);
        }
    }
}
