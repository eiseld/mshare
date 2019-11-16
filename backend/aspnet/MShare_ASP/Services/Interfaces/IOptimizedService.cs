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

        /// <summary>Recalculate the optimal debt for a specific group</summary>
        Task OptimizeForGroup(long groupId);
    }
}