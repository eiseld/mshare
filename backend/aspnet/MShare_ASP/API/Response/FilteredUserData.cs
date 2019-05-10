using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response
{
	public class FilteredUserData
	{

		/// <summary>
		/// Id of the user
		/// </summary>
		public long Id { get; set; }

		/// <summary>
		/// DisplayName of the user
		/// </summary>
		public string DisplayName { get; set; }

		/// <summary>
		/// Email of the user
		/// </summary>
		public string Email { get; set; }

	}

}
