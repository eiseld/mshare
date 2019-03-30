using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    /// <summary>
    /// General service for Mshare Application
    /// </summary>
    public interface IMshareService {
        /// <summary>
        /// Gets a specific user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DaoUser> GetUser(long id);

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        Task<IList<DaoUser>> GetUsers();
    }
}
