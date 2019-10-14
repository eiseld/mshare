using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    /// <summary>Optimized debt related services</summary>
    interface IOptimizedService
    {

#if DEBUG

        /// <summary>Recalculate the optimal debt for all groups (DEBUG ONLY)</summary>
        Task OptimizeForAllGroup();

        /// <summary>Recalculate the optimal debt for a specific group (DEBUG ONLY)</summary>
        Task OptimizeForGroup(long groupId);

#endif

        /// <summary>Todo</summary>
        Task OptimizeForRemoveMember(long groupId);

        /// <summary>Todo</summary>
        Task OptimizeForAddSpending(long groupId);

        /// <summary>Todo</summary>
        Task OptimizeForModifySpending(long groupId);

        /// <summary>Todo</summary>
        Task OptimizeForRemoveSpending(long groupId);

        /// <summary>Todo</summary>
        Task OptimizeForSettling(long groupId);
    }
}
