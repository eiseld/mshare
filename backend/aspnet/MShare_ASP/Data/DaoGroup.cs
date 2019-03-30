using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for Group
    /// </summary>
    [Table("groups", Schema = "mshare")]
    public class DaoGroup {
        /// <summary>
        /// Primary key for Group
        /// </summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// Name of the group
        /// </summary>
        [Column("name")]
        public String Name { get; set; }
        /// <summary>
        /// Id of the creator of this Group
        /// </summary>
        [Column("creator_user_id")]
        public long CreatorUserId { get; set; }

        /// <summary>
        /// The creator of this group
        /// </summary>
        [JsonIgnore]
        [ForeignKey("CreatorUserId")]
        public virtual DaoUser CreatorUser { get; set; }

        /// <summary>
        /// All Users associted with this Group
        /// </summary>
        public IEnumerable<DaoUsersGroupsMap> Members { get; set; }
    }
}
