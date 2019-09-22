using System.Threading.Tasks;

namespace MShare_ASP.Services
{

    /// <summary>Logging related services</summary>
    public interface ILoggingService
    {

        /// <summary>Log and associate with a group</summary>
        [System.Obsolete("Don't use this!")] Task LogForGroup(long userId, long groupId, object data);
    }
}