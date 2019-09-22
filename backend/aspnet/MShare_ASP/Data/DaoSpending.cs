using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{

    /// <summary>Data Access Object for Spendings</summary>
    [Table("spendings", Schema = "mshare")]
    public class DaoSpending
    {

        /// <summary>Unique Id of this spending</summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>Name of the spending</summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>Amount of money that has been spent</summary>
        [Column("money_owed")]
        public long MoneyOwed { get; set; }

        /// <summary>Id of the creditor user</summary>
        [Column("creditor_user_id")]
        public long CreditorUserId { get; set; }

        /// <summary>The group's id from which this spending is originated</summary>
        [Column("group_id")]
        public long GroupId { get; set; }

        /// <summary>Creditor user</summary>
        [JsonIgnore]
        [ForeignKey("CreditorUserId")]
        public virtual DaoUser Creditor { get; set; }

        /// <summary>Group of this spending</summary>
        [JsonIgnore]
        [ForeignKey("GroupId")]
        public virtual DaoGroup Group { get; set; }

        /// <summary>List of all the debtors this spending includes</summary>
        public virtual IList<DaoDebtor> Debtors { get; set; }
    }
}
