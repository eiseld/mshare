using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for Log Types</summary>
    [Table("log_types", Schema = "mshare")]
    public class DaoLogType
    {
        /// <summary>Primary key of this type</summary>
        [Key]
        [Column("id")]
        public short Id { get; set; }

        /// <summary>English name of the type (e.g.: insert, update, delete)</summary>
        [Column("type_name")]
        public string TypeName { get; set; }

        /// <summary>Log Type describes the kind of operation this log is associated with</summary>
        public enum Type
        {
            /// <summary>Update - Anything that updated an already existing data</summary>
            UPDATE = 1,
            /// <summary>Create - Anything that creates new data</summary>
            CREATE = 2,
            /// <summary>Delete - Anything that deleted data from the database</summary>
            DELETE = 3,
            /// <summary>Add - Anything that adds to an already existing collection</summary>
            ADD = 4,
            /// <summary>Remove - Anything that removes from an already existing collection</summary>
            REMOVE = 5
        }
    }
}
