using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    /// <summary>
    /// Service containing logging related methods
    /// </summary>
    public interface ILoggingService {
        /// <summary>
        /// Log and associate with a group
        /// </summary>
        /// <param name="userId">the Id of the logger</param>
        /// <param name="groupId">the group to log to</param>
        /// <param name="data">the data to associate with this log</param>
        Task LogForGroup(long userId, long groupId, object data);
    }
}