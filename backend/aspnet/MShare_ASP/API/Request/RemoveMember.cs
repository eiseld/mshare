using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Request{

    /// <summary>
    /// Represents a RemoveMember request
    /// </summary>
    public class RemoveMember{
        /// <summary>
        /// User Id of the to be removed member
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// Validator object for RemoveMember data class
    /// </summary>
    public class RemoveMemberValidator : AbstractValidator<RemoveMember>{

        /// <summary>
        /// Initializese the validator object
        /// </summary>
        public RemoveMemberValidator(){
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }

}
