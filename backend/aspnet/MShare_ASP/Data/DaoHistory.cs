using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for History
    /// </summary>
    [Table("history", Schema = "mshare")]
    public class DaoHistory {
        /// <summary>
        /// Primary key of the history
        /// </summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }
        /// <summary>
        /// Group id of the history
        /// </summary>
        [Column("groupid")]
        public long GroupId { get; set; }
        /// <summary>
        /// User id of the history
        /// </summary>
        [Column("userid")]
        public long UserId { get; set; }
        /// <summary>
        /// Date of the history
        /// </summary>
        [Column("date")]
        public DateTime Date { get; set; }
        /// <summary>
        /// Balance of the history
        /// </summary>
        [Column("balance")]
        public int CreationDate { get; set; }
    }
}