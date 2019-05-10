using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services.Exceptions {
    /// <summary>
    /// Equivalent to HttpStatusCode.InternalServerError, 500
    /// </summary>
    public class DatabaseException : Exception {
        internal DatabaseException(string message = "") :
            base(message) {
        }
    }
}
