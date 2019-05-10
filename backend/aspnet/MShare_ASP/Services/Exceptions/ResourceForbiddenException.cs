using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services.Exceptions {
    /// <summary>
    /// Equivalent to HttpStatusCode.Forbidden, 403
    /// </summary>
    public class ResourceForbiddenException : Exception {
        internal ResourceForbiddenException(string message = "") :
            base(message) {
        }
    }
}
