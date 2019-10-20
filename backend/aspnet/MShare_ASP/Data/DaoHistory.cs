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

        /// <summary>History creator</summary>
        [Column("acting_user_id")]
        public long UserId { get; set; }
        
        /// <summary>Affected entity id</summary>
        [Column("affected_id")]
        public long AffectedId { get; set; }

        /// <summary>Date of the history</summary>
        [Column("date")]
        public DateTime? Date { get; set; }

        /// <summary>Type of this history</summary>
        [EnumDataType(typeof(DaoLogType.Type))]
        [Column("type")]
        public DaoLogType.Type Type { get; set; }

        /// <summary>SubType of this history</summary>
        [EnumDataType(typeof(DaoLogSubType.Type))]
        [Column("subtype")]
        public DaoLogSubType.Type SubType { get; set; }

        /// <summary>Log that belongs to this history</summary>
        [Column("log")]
        public string SerializedLog { get; set; }
    }
}