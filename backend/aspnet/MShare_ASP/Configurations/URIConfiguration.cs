using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    public interface IURIConfiguration {
        String URIForEndUsers { get; }
    }
    public class URIConfiguration : IURIConfiguration {
        public String URIForEndUsers { get; set; }
    }
}
