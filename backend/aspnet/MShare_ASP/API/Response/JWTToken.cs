using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response {
    /// <summary>
    /// Encapsulates a JWT token https://jwt.io
    /// </summary>
    public class JWTToken {
        /// <summary>
        /// String representation of the token
        /// </summary>
        public String Token { get; set; }
    }
}
