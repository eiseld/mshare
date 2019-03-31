using MShare_ASP.API.Request;
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

        /// <summary>
        /// Returns all groups
        /// </summary>
        /// <returns></returns>
        Task<IList<DaoGroup>> GetGroups();

        /// <summary>
        /// Returns the group with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DaoGroup> GetGroup(long id);

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="newGroup"></param>
        /// <param name="forUser"></param>
        /// <returns></returns>
        Task CreateGroup(NewGroup newGroup, long forUser);

        /// <summary>
        /// Sends the forgotten password email to the user
        /// NOTE: Should always return 200!
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task SendForgotPasswordMail(API.Request.ValidEmail email);

        /// <summary>
        /// Updates the password of the user
        /// </summary>
        /// <param name="passwordUpdate"></param>
        /// <returns></returns>
        Task UpdatePassword(API.Request.PasswordUpdate passwordUpdate);
    }
}
