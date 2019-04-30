using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response{

    /// <summary>
    ///  Describes an optimal debt
    /// </summary>
    public class OptimisedDebtData{

        /// <summary>
        /// Id of the Debtor
        /// </summary>
        public UserData Debtor { get; set; }

        /// <summary>
        /// Id of the Creditor
        /// </summary>
        public UserData Creditor { get; set; }

        /// <summary>
        /// The amount of the optimised debt 
        /// </summary>
        public long OptimisedDebtAmount { get; set; }
    }
}
