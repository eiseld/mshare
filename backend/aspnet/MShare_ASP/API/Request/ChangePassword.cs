using FluentValidation;
using MShare_ASP.Data;
using System;
using System.Linq;

namespace MShare_ASP.API.Request
{
	/// <summary>Represents a ChangePassword request</summary>
	public class ChangePassword
	{
		/// <summary>Old password</summary>
		public String OldPassword { get; set; }

		/// <summary>New password</summary>
		public String NewPassword { get; set; }

		/// <summary>Id of the user</summary>
		public long Id { get; set; }

		/// <summary> 2 character representation of the language </summary>
		public DaoLangTypes.Type Lang { get; set; }
	}

	/// <summary>Validator object for ChangePassword data class</summary>
	public class ChangePasswordValidator : AbstractValidator<ChangePassword>
	{
		/// <summary>Initializes the validator object</summary>
		public ChangePasswordValidator()
		{
			RuleFor(x => x.NewPassword)
				.Must(x => x.Any(char.IsLower))
				.WithMessage("Must have a lower case letter!")
				.Must(x => x.Any(char.IsUpper))
				.WithMessage("Must have an upper case letter!")
				.Must(x => x.Any(char.IsDigit))
				.WithMessage("Must have at least one digit!")
				.MinimumLength(6);

			RuleFor(x => x.Lang)
				.IsInEnum()
				.WithMessage("Specified language is not supported by the server");
		}
	}
}