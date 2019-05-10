using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services.Exceptions {
    /// <summary>
    /// Equivalent to HttpStatusCode.Conflict, 409
    /// </summary>
    public class BusinessException : Exception {
        internal BusinessException(string message = "") :
            base(message) {
        }
    }
}
