using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
	/// <summary>
	/// Data Access Object for Debts
	/// </summary>
	[Table("debts", Schema = "mshare")]
	public class DaoDebt
	{
		/// <summary>
		/// Primary key for Debts
		/// </summary>
		[Key]
		[Column("id")]
		public long Id { get; set; }
		/// <summary>
		/// Id of the debtor
		/// </summary>
		[Column("deptorid")]
		public long DebtorId { get; set; }
		/// <summary>
		/// Id of the lender
		/// </summary>
		[Column("lenderid")]
		public long LenderId { get; set; }
		/// <summary>
		/// Id of the group
		/// </summary>
		[Column("groupid")]
		public long GroupId { get; set; }
		/// <summary>
		/// Amount of the debt
		/// </summary>
		[Column("amount")]
		public long Amount { get; set; }
	}
}
