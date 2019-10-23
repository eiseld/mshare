using MShare_ASP.API.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    /// <summary>Optimized debt related services</summary>
    public interface IOptimizedService
    {
#if DEBUG

        /// <summary>Recalculate the optimal debt for all groups (DEBUG ONLY)</summary>
        Task OptimizeForAllGroup();

#endif

        /// <summary>Recalculate the optimal debt for a specific group (DEBUG ONLY)</summary>
        Task OptimizeForGroup(long groupId);

        /// <summary>Updates OptimizedDebts after adding a new spending</summary>
        Task OptimizeForNewSpending(long userId, NewSpending newSpending);

        /// <summary>Updates OptimizedDebts after updating a spending</summary>
        Task OptimizeForUpdateSpending(long groupId, long creditorId, Dictionary<long, long> oldDebts, Dictionary<long, long> newDebts);

        /// <summary>Updates OptimizedDebts after a settlement</summary>
        Task OptimizeForSettling(long groupId, long creditorId, long debtorId);
    }
}