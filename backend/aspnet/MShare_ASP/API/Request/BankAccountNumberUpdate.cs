using FluentValidation;
using System;
using System.Linq;

namespace MShare_ASP.API.Request
{
    /// <summary>Represents a BankAccountNumberUpdate request</summary>
    public class BankAccountNumberUpdate
    {
        /// <summary>New bank account number of the user</summary>
        public String BankAccountNumber { get; set; }
    }

    /// <summary>Validator object for BankAccountNumberUpdate data class</summary>
    public class BankAccountNumberValidator : AbstractValidator<BankAccountNumberUpdate>
    {
        /// <summary>Initializes the validator object</summary>
        public BankAccountNumberValidator()
        {
            RuleFor(x => x.BankAccountNumber)
                .Must(x => x.All(char.IsDigit))
                .WithMessage("Must be digits only!")
                .Length(24);
        }
    }
}