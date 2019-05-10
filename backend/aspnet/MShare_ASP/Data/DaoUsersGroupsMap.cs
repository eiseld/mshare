using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Junction table for Users and Groups many-to-many connection
    /// </summary>
    [Table("users_groups_map", Schema = "mshare")]
    public class DaoUsersGroupsMap {
        /// <summary>
        /// Composite key of this table
        /// </summary>
        [Column("user_id")]
        public long UserId { get; set; }
        /// <summary>
        /// Composite key of this table
        /// </summary>
        [Column("group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// User contained in this junction
        /// </summary>
        [ForeignKey("UserId")]
        public virtual DaoUser User { get; set; }
        /// <summary>
        /// Group contained in this junction
        /// </summary>
        [ForeignKey("GroupId")]
        public virtual DaoGroup Group { get; set; }
    }
}
