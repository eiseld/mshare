using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services.Exceptions {
    /// <summary>
    /// Equivalent to HttpStatusCode.Gone, 410
    /// </summary>
    public class ResourceGoneException : Exception {
        internal ResourceGoneException(string message = "") :
            base(message) {
        }
    }
}
