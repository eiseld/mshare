using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for EmailToken</summary>
    [Table("optimized_debt", Schema = "mshare")]
    public class DaoOptimizedDebt
    {
        /// <summary>Foreign key to the group</summary>
        [Column("group_id")]
        public long GroupId { get; set; } = 0;

        /// <summary>Foreign key to the owing user</summary>
        [Column("user_owes_id")]
        public long UserOwesId { get; set; } = 0;

        /// <summary>Foreign key to the owed user</summary>
        [Column("user_owed_id")]
        public long UserOwedId { get; set; } = 0;

        /// <summary>Amount owed</summary>
        [Column("owe_amount")]
        public long OweAmount { get; set; }

        /// <summary>The group inwhich the the users owe one another.</summary>
        [JsonIgnore]
        [ForeignKey("GroupId")]
        public virtual DaoGroup Group { get; set; }

        /// <summary>The user that owes the other one.</summary>
        [JsonIgnore]
        [ForeignKey("UserOwesId")]
        public virtual DaoUser UserOwes { get; set; }

        /// <summary>The user that is owed by the other one.</summary>
        [JsonIgnore]
        [ForeignKey("UserOwedId")]
        public virtual DaoUser UserOwed { get; set; }
    }
}