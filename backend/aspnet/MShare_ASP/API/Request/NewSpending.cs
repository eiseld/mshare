using FluentValidation;
using System.Linq;

namespace MShare_ASP.API.Request
{

    /// <summary>Describes the structure of the request for creating a new spending</summary>
    public class NewSpending
    {

        /// <summary>Debtor structure that should be added for this Spending</summary>
        public class Debtor
        {

            /// <summary>Id of the debtor</summary>
            public long DebtorId { get; set; }

            /// <summary>
            /// Debt owed by this debtor
            /// Be aware: the sum of all of the debts, must be equal to MoneySpent!
            /// Note: calculate it client side
            /// </summary>
            public long? Debt { get; set; }
        }

        /// <summary>Group id of the spending to be added to</summary>
        public long GroupId { get; set; }

        /// <summary>Name of the spending</summary>
        public string Name { get; set; }

        /// <summary>Amount of money that has been spent</summary>
        public long MoneySpent { get; set; }

        /// <summary>List of debtors, always specify this</summary>
        public Debtor[] Debtors { get; set; }
    }

    /// <summary>Validator object for NewSpending's Debtor subclass</summary>
    public class NewSpending_DebtorValidator : AbstractValidator<NewSpending.Debtor>
    {

        /// <summary>Initializes the validator object</summary>
        public NewSpending_DebtorValidator()
        {
            RuleFor(x => x.DebtorId)
                .NotEmpty();
            RuleFor(x => x.Debt)
                .NotEmpty()
                .GreaterThan(0)
                //.When(x => x.Debt != null)
                .WithMessage("Debt should not be 0, either don't add debtor to list or if you want to use auto generated values, don't pass 'Debt' field in!");
        }
    }

    /// <summary>Validator object for NewUser data class</summary>
    public class NewSpendingValidator : AbstractValidator<NewSpending>
    {

        /// <summary>Initializes the validator object</summary>
        public NewSpendingValidator()
        {
            RuleFor(x => x.MoneySpent)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(x => x.GroupId)
                .NotEmpty();

            RuleForEach(x => x.Debtors)
                .SetValidator(new NewSpending_DebtorValidator());

            //RuleFor(x => x.Debtors)
            //    // Specified debt must be validated so that remainder can be distributed between automatic debtors
            //    .Must((args, d) => d.Sum(m => m.Debt ?? 0) <= args.MoneySpent - d.Count(x => !x.Debt.HasValue))
            //        // Only check previous must if Debtors is not empty
            //        .When(x => x.Debtors.Any() && !x.Debtors.All(d => d.Debt.HasValue))
            //        .WithMessage("Sparsely specified debts sum is more than MoneySpent - count of unspecified");

            RuleFor(x => x.Debtors)
                .NotEmpty()
                // If we specify all debts it must equal exactly the MoneySpent
                .Must((args, d) => d.Sum(m => m.Debt ?? 0) == args.MoneySpent)
                    // Only check previous must if all Debts are assigned
                    //.When(x => x.Debtors.Any() && x.Debtors.All(d => d.Debt.HasValue))
                    .WithMessage("Fully specified debts sum is not equal to MoneySpent");
        }
    }
}
