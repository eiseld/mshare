using MShare_ASP.API.Request;
using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    /// <summary>
    /// Spending related services
    /// </summary>
    public interface ISpendingService {
        /// <summary>
        /// Gets all of the spendings with its internal data for a specifis group
        /// </summary>
        /// <param name="id">group ID</param>
        Task<IList<DaoSpending>> GetSpendingsForGroup(long id);
        /// <summary>
        /// Converts spending internal data to facing data
        /// </summary>
        /// <param name="spendings"></param>
        /// <returns></returns>
        IList<API.Response.SpendingData> ToSpendingData(IList<DaoSpending> spendings);
        /// <summary>
        /// Creates a new spending in the database
        /// </summary>
        /// <param name="newSpending">the syntactically validated new spending request</param>
        /// <param name="userId">creator of the spending</param>
        /// <returns></returns>
        Task CreateNewSpending(NewSpending newSpending, long userId);

        Task<IList<DaoOptimizedDebt>> GetOptimizedDebtForGroup(long groupid);
    }
}