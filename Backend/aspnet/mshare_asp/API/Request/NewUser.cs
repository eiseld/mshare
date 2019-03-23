using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Request {
    /// <summary>
    /// Represents a new user to be registered
    /// </summary>
    public class NewUser {
        /// <summary>
        /// Email of the user as in SMTP standard
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Name to be displayed
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Unhashed password
        /// </summary>
        public String Password { get; set; }
    }
}
