using System.Collections.Generic;

namespace MShare_ASP.API.Response
{
    /// <summary>Facing Data for a specific spending</summary>
    public class SpendingData
    {
        /// <summary>Id of the spending</summary>
        public long Id { get; set; }

        /// <summary>Name of the spending</summary>
        public string Name { get; set; }

        /// <summary>Name of the spending</summary>
        public UserData Creditor { get; set; }

        /// <summary>Amount of money that has been spent</summary>
        public long MoneyOwed { get; set; }

        /// <summary>Id of the creditor user</summary>
        public long CreditorUserId { get; set; }

        /// <summary>Debtors of this spending</summary>
        public IList<DebtorData> Debtors { get; set; }

        /// <summary>Date of the spending</summary>
        public string Date { get; set; }
    }
}