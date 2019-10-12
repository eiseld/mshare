using FluentValidation;
using MShare_ASP.Data;

namespace MShare_ASP.API.Request
{
    /// <summary> Represents a Language setting request </summary>
    public class SetLang
    {
        /// <summary> 2 character representation of the language </summary>
        public DaoLangTypes.Type Lang { get; set; }
    }

    /// <summary> Validator object for Email class </summary>
    public class SetLangValidator : AbstractValidator<SetLang>
    {
        /// <summary> Initializese the validator object </summary>
        public SetLangValidator()
        {
            RuleFor(x => x.Lang)
                .IsInEnum()
                .WithMessage("Specified language is not supported by the server");
        }
    }
}