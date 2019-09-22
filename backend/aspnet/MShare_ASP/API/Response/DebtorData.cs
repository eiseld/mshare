using System;

namespace MShare_ASP.API.Response
{

    /// <summary>Facing Data for a specific debtor</summary>
    public class DebtorData
    {

        /// <summary>Id of the debtor</summary>
        public long Id { get; set; }

        /// <summary>Max 32 length name of the member</summary>
        public String Name { get; set; }

        /// <summary>Member's debt</summary>
        public long Debt { get; set; }
    }
}
