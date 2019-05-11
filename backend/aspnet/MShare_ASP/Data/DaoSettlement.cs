using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data
{
	/// <summary>
	/// Data Access Object for Settlements
	/// </summary>
	[Table("settlements", Schema = "mshare")]
	public class DaoSettlement
	{

		/// <summary>
		/// Unique Id of this settlement
		/// </summary>
		[Key]
		[Column("id")]
		public long Id { get; set; }

		/// <summary>
		/// The group's id to where this settlement is payed
		/// </summary>
		[Column("group_id")]
		public long GroupId { get; set; }

		/// <summary>
		/// Id of the debtor who pays this settlement
		/// </summary>
		[Column("from")]
		public long From { get; set; }

		/// <summary>
		/// Id of the lender who receives this settlement
		/// </summary>
		[Column("to")]
		public long To { get; set; }

		/// <summary>
		/// Amount of money that needs to be settled
		/// </summary>
		[Column("amount")]
		public long Amount { get; set; }

	}

}
