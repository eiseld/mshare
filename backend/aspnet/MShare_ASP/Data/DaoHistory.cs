using MShare_ASP.Services.Exceptions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        /// <summary>GroupId</summary>
        [Column("group_id")]
        public long? GroupId { get; set; }

        /// <summary>Affected entity ids</summary>
        [NotMapped]
        public long[] AffectedIds {
            get {
                return __AffectedIs
                    .Split(",",StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => {
                        if (long.TryParse(x, out long result))
                            return result;
                        else
                            throw new DatabaseException($"AffectedID list not formatted properly in history table at id: {Id}");
                    })
                    .ToArray();
            }
            set {
                __AffectedIs = String.Join(",", value.Select(x => x.ToString()));
            }
        }

        /// <summary>Affected entity ids in string format</summary>
        [Column("affected_ids")]
        public string __AffectedIs { get; set; }

        /// <summary>Date of the history</summary>
        [Column("date")]
        public DateTime Date { get; set; }

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