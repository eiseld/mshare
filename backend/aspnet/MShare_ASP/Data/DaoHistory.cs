using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for History</summary>
    [Table("history", Schema = "mshare")]
    public class DaoHistory
    {
        /// <summary>Primary key of the history</summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>Associated group</summary>
        [Column("groupid")]
        public long GroupId { get; set; }

        /// <summary>History creator</summary>
        [Column("userid")]
        public long UserId { get; set; }

        /// <summary>Date of the history</summary>
        [Column("date")]
        public DateTime? Date { get; set; }

        /// <summary>Log that belongs to this history</summary>
        [Column("log")]
        public string SerializedLog { get; set; }
    }
}