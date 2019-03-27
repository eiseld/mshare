using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    public interface IJWTConfiguration {
        string SecretKey { get; }
    }
    public class JWTConfiguration : IJWTConfiguration {
        public string SecretKey { get; set; }
    }
}
