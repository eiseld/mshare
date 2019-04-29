using FluentValidation;

namespace MShare_ASP.API.Request
{

	/// <summary>
	/// Represents a RemoveMember request
	/// </summary>
	public class AddMember
	{
		/// <summary>
		/// User Id of the to be removed member
		/// </summary>
		public long Id { get; set; }
	}

	/// <summary>
	/// Validator object for RemoveMember data class
	/// </summary>
	public class AddMemberValidator : AbstractValidator<RemoveMember>
	{

		/// <summary>
		/// Initializese the validator object
		/// </summary>
		public AddMemberValidator()
		{
			RuleFor(x => x.Id)
				.NotEmpty();
		}
	}

}
