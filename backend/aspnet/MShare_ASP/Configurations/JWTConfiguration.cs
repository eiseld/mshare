using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    /// <summary>
    /// Configuration for JWT token generation and validation
    /// </summary>
    public interface IJWTConfiguration {
        /// <summary>
        /// Secret key to encode tokens with
        /// </summary>
        string SecretKey { get; }
    }

    internal class JWTConfiguration : IJWTConfiguration {
        public string SecretKey { get; set; }
    }
}
