using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response {
<<<<<<< HEAD
    /// <summary>
    /// Facing Data for a specific debtor
    /// </summary>
=======
>>>>>>> dd3d8b4... Added spending creation (post) and list (get) API requests
    public class DebtorData {
        /// <summary>
        /// Id of the debtor
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Max 32 length name of the member
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Member's debt
        /// </summary>
        public long Debt { get; set; }
    }
}
