using System;

namespace MShare_ASP.API.Response
{
    /// <summary>Describes the group member's data</summary>
    public class MemberData
    {
        /// <summary>Id of the member</summary>
        public long Id { get; set; }

        /// <summary>Max 32 length name of the member</summary>
        public String Name { get; set; }

        /// <summary>Member balance in the view of the group</summary>
        public long Balance { get; set; }

        /// <summary>Bank account number of the member</summary>
        public String BankAccountNumber { get; set; }
    }
}