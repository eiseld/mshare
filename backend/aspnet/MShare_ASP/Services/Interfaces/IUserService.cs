using MShare_ASP.API.Request;
using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services{

    /// <summary>
    /// User related services
    /// </summary>
    public interface IUserService{

        /// <summary>
        /// Converts DaoUser to UserData
        /// </summary>
        /// <param name="daoUser"></param>
        /// <returns></returns>
        API.Response.UserData ToUserData(DaoUser daoUser);

        /// <summary>
        /// Converts list of DaoUser to list of UserData
        /// </summary>
        /// <param name="daoUsers"></param>
        /// <returns></returns>
        IList<API.Response.UserData> ToUserData(IList<DaoUser> daoUsers);

        /// <summary>
        /// Gets a specific user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<DaoUser> GetUser(long user);

        /// <summary>
        /// Returns all users
        /// </summary>
        /// <returns></returns>
        Task<IList<DaoUser>> GetUsers();

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