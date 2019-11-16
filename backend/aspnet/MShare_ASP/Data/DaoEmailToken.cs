using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for EmailToken</summary>
    [Table("email_tokens", Schema = "mshare")]
    public class DaoEmailToken
    {
        /// <summary>Determines the type of an email</summary>
        public enum Type
        {
            /// <summary>Used for password related (e.g. forgotten password) emails</summary>
            Password = 1,

            /// <summary>Used for email validation (when registering)</summary>
            Validation = 2
        }

        /// <summary>Foreign key to the user</summary>
        [Column("user_id")]
        public long UserId { get; set; } = 0;

        /// <summary>Primary key of the emailtoken</summary>
        [Column("token")]
        public string Token { get; set; }

        /// <summary>Date and time of expiration</summary>
        [Column("expiration_date")]
        public DateTime ExpirationDate { get; set; }

        /// <summary>Type of token</summary>
        [EnumDataType(typeof(Type))]
        [Column("token_type")]
        public Type TokenType { get; set; }

        /// <summary>The user associated with this EmailToken</summary>
        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual DaoUser User { get; set; }
    }
}