using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for EmailToken
    /// </summary>
    public class DaoEmailToken {
        /// <summary>
        /// Determines the type of an email
        /// </summary>
        public enum Type {
            /// <summary>
            /// Used for password related (e.g. forgotten password) emails
            /// </summary>
            Password = 1,
            /// <summary>
            /// Used for email validation (when registering)
            /// </summary>
            Validation = 2
        }
        /// <summary>
        /// Foreign key to the user
        /// </summary>
        public long user_id { get; set; } = 0;
        /// <summary>
        /// Primary key of the emailtoken
        /// </summary>
        [Key]
        public String token { get; set; }
        /// <summary>
        /// Date and time of expiration
        /// </summary>
        public DateTime expiration_date { get; set; }
        /// <summary>
        /// Type of token
        /// </summary>
        [EnumDataType(typeof(Type))]
        public Type token_type { get; set; }

        /// <summary>
        /// The user associated with this EmailToken
        /// </summary>
        [JsonIgnore]
        [ForeignKey("user_id")]
        public virtual DaoUser User { get; set; }
    }
}
