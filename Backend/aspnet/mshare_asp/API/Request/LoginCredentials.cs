using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Request {
    /// <summary>
    /// Identifies the user
    /// </summary>
    public class LoginCredentials {
        /// <summary>
        /// Email that the user registered with
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Unhashed password
        /// </summary>
        public String Password { get; set; }
    }
}
