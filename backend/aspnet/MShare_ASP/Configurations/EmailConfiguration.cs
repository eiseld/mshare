using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    public interface IEmailConfiguration {
        string Name { get; }
        string Address { get; }
        string Password { get; }

        string SmtpAddress { get; }
        int SmtpPort { get; }
    }
    public class EmailConfiguration : IEmailConfiguration {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
    }
}
