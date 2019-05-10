using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Configurations {
    /// <summary>
    /// URI related configurations go here
    /// </summary>
    public interface IURIConfiguration {
        /// <summary>
        /// URI that the end user sees (like in an email)
        /// </summary>
        String URIForEndUsers { get; }
    }
    internal class URIConfiguration : IURIConfiguration {
        public String URIForEndUsers { get; set; }
    }
}
