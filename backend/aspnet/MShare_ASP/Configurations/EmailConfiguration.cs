using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    /// <summary>
    /// Configurations for an email account
    /// </summary>
    public interface IEmailConfiguration {
        /// <summary>
        /// Name of the account (e.g. Mshare Noreply)
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Email Address of the account (e.g. noreply.mshare@gmail.com)
        /// </summary>
        string Address { get; }
        /// <summary>
        /// Password for the email address
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Address of the SMTP server
        /// </summary>
        string SmtpAddress { get; }

        /// <summary>
        /// Port used with SMTP server
        /// </summary>
        int SmtpPort { get; }
    }

    internal class EmailConfiguration : IEmailConfiguration {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }

        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
    }
}
