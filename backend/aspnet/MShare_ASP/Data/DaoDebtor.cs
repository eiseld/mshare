using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for Debtors
    /// </summary>
    [Table("debtors", Schema = "mshare")]
    public class DaoDebtor {
        /// <summary>
        /// Id of the spending's id associated with this debt
        /// </summary>
        [Column("spending_id")]
        public long SpendingId { get; set; }
        /// <summary>
        /// Debtor's id associated with this debt
        /// </summary>
        [Column("debtor_user_id")]
        public long DebtorUserId { get; set; }
        /// <summary>
        /// Debt of this debtor
        /// </summary>
        [Column("debt")]
        public long? Debt { get; set; }
        /// <summary>
        /// Spending contained in this junction
        /// </summary>
        [ForeignKey("SpendingId")]
        [JsonIgnore]
        public virtual DaoSpending Spending { get; set; }
        /// <summary>
        /// Debtor contained in this junction
        /// </summary>
        [ForeignKey("DebtorUserId")]
        [JsonIgnore]
        public virtual DaoUser Debtor { get; set; }
    }
}
