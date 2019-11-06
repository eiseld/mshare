using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for Log SubTypes</summary>
    [Table("log_subtypes", Schema = "mshare")]
    public class DaoLogSubType
    {
        /// <summary>Primary key of this subtype</summary>
        [Key]
        [Column("id")]
        public short Id { get; set; }

        /// <summary>English name of the subtype (e.g.: spending, debt)</summary>
        [Column("subtype_name")]
        public string SubTypeName { get; set; }

        /// <summary>Log SubTypes describe what domain this log is associated with</summary>
        public enum Type
        {
            /// <summary>Spending - anything that modifies the spending itself (name, price)</summary>
            SPENDING = 1,

            /// <summary>Settlement - anything that is associated with a settlement (paying off debt)</summary>
            SETTLEMENT = 2,

            /// <summary>Debt - anything that is associated with a debt (creating new debt)</summary>
            DEBT = 3,

            /// <summary>Group - anything that modifies the group itself (owner, name, icon, etc)</summary>
            GROUP = 4,

            /// <summary>Member - when adding / removing members to a group</summary>
            MEMBER = 5
        }
    }
}