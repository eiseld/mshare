using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data {
    public class DaoEmailToken {
        public enum Type {
            Password = 1,
            Validation = 2
        }
        public long user_id { get; set; } = 0;
        [Key]
        public String token { get; set; }
        public DateTime expiration_date { get; set; }
        [EnumDataType(typeof(Type))]
        public Type token_type { get; set; }

        [JsonIgnore]
        [ForeignKey("user_id")]
        public virtual DaoUser User { get; set; }
    }
}
