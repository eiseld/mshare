using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response {
    /// <summary>
    /// Describes the user' data
    /// </summary>
    public class UserData {
        /// <summary>
        /// Max 32 length name of the user
        /// </summary>
        public String DisplayName { get; set; }
    }
}
