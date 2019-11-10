using FluentValidation;
using System.Linq;

namespace MShare_ASP.API.Request
{
    /// <summary>Describes the structure of the request for updating an existing spending</summary>
    public class SpendingUpdate
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
            public long Debt { get; set; }
        }

        /// <summary>Group id of the spending to be added to</summary>
        public long GroupId { get; set; }

        /// <summary>Name of the spending</summary>
        public string Name { get; set; }

        /// <summary>Id of the spending</summary>
        public long Id { get; set; }

        /// <summary>Id of the creditor user</summary>
        public long CreditorUserId { get; set; }

        /// <summary>Amount of money that has been spent</summary>
        public long MoneySpent { get; set; }

        /// <summary>List of debtors, always specify this</summary>
        public Debtor[] Debtors { get; set; }

        /// <summary>Date of the spending</summary>
        public string Date { get; set; }
    }

    /// <summary>Validator object for SpendingUpdate's Debtor subclass</summary>
    public class SpendingUpdate_DebtorValidator : AbstractValidator<SpendingUpdate.Debtor>
    {
        /// <summary>Initializes the validator object</summary>
        public SpendingUpdate_DebtorValidator()
        {
            RuleFor(x => x.DebtorId)
                .NotEmpty();

            RuleFor(x => x.Debt)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Debt should not be 0");
        }
    }

    /// <summary>Validator object for SpendingUpdate data class</summary>
    public class SpendingUpdateValidator : AbstractValidator<SpendingUpdate>
    {
        /// <summary>Initializes the validator object</summary>
        public SpendingUpdateValidator()
        {
            RuleFor(x => x.MoneySpent)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(x => x.GroupId)
                .NotEmpty();

            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.CreditorUserId)
                .NotEmpty();

            RuleForEach(x => x.Debtors)
                .SetValidator(new SpendingUpdate_DebtorValidator())
                .Must((context, current) => context.Debtors.Count(x => x.DebtorId == current.DebtorId) == 1);

            RuleFor(x => x.Debtors)
                .NotEmpty()
                .Must((args, d) => d.Sum(m => m.Debt) == args.MoneySpent)
                    .WithMessage("Fully specified debts sum is not equal to MoneySpent");

            RuleFor(x => x.Date)
                .Matches("\\b[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}Z\\b")
                .When(x => x.Date != "")
                .NotNull();
        }
    }
}