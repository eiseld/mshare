using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    public class DaoUser {
        [Key]
        public long id { get; set; }
        public String email { get; set; }
        public String password { get; set; }
        public String display_name { get; set; }
        public DateTime creation_date { get; set; }

        public IEnumerable<DaoEmailToken> email_tokens { get; set; }
    }
}