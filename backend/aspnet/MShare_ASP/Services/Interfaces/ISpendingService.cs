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
        /// Converts spending internal data to facing data
        /// </summary>
        /// <param name="spendings"></param>
        /// <returns></returns>
        IList<API.Response.SpendingData> ToSpendingData(IList<DaoSpending> spendings);

        /// <summary>
        /// Converts optimized debts internal data to facing data
        /// </summary>
        /// <param name="optimizedDebts"></param>
        /// <returns></returns>
        IList<API.Response.OptimisedDebtData> ToOptimisedDebtData(IList<DaoOptimizedDebt> optimizedDebts);

        /// <summary>
        /// Gets all of the spendings with its internal data for a specifis group
        /// </summary>
        /// <param name="groupId"></param>
        Task<IList<DaoSpending>> GetSpendingsForGroup(long groupId);

        /// <summary>
        /// Creates a new spending in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<IList<DaoOptimizedDebt>> GetOptimizedDebtForGroup(long userId, long groupId);

        /// <summary>
        /// Calculate the optimized debts of a group
        /// </summary>
        /// <param name="groupId">creator of the spending</param>
        /// <returns></returns>
        Task OptimizeSpendingForGroup(long groupId);

        /// <summary>
        /// Creates a new spending in the database
        /// </summary>
        /// <param name="newSpending">the syntactically validated new spending request</param>
        /// <param name="userId">creator of the spending</param>
        /// <returns></returns>
        Task CreateNewSpending(NewSpending newSpending, long userId);

    }
}