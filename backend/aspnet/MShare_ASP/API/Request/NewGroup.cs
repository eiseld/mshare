using FluentValidation;
using System;

namespace MShare_ASP.API.Request
{

    /// <summary>Represents a new group to be registered</summary>
    public class NewGroup
    {

        /// <summary>Name of the group to be created</summary>
        public String Name { get; set; }
    }

    /// <summary>Validator object for NewUser data class</summary>
    public class NewGroupValidator : AbstractValidator<NewGroup>
    {

        /// <summary>Initializes the validator object</summary>
        public NewGroupValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(32);
        }
    }
}
