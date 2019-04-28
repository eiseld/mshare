using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for User
    /// </summary>
    [Table("history", Schema = "mshare")]
    public class DaoHistory {
        /// <summary>
        /// Primary key of the user
        /// </summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// Email of the user
        /// </summary>
        [Column("groupid")]
        public long GroupId { get; set; }
        /// <summary>
        /// Hashed password of the user
        /// </summary>
        [Column("userid")]
        public long UserId { get; set; }
        /// <summary>
        /// Displayname (not unique!)
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }
        /// <summary>
        /// Date and time when the user was registered
        /// </summary>
        [Column("balance")]
        public int CreationDate { get; set; }
    }
}