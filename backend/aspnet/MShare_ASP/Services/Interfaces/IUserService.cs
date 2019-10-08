using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Services
{

    /// <summary>User related services</summary>
    public interface IUserService
    {

        /// <summary>Converts user data from internal to facing</summary>
        UserData ToUserData(DaoUser daoUser);

        /// <summary>Converts multiple user data from internal to facing</summary>
        IList<UserData> ToUserData(IList<DaoUser> daoUsers);

        /// <summary>Gets user with given ID</summary>
        /// <exception cref="ResourceNotFoundException">["user"]</exception>
        Task<DaoUser> GetUser(long userId);

#if DEBUG
        /// <summary>Gets all user (DEBUG ONLY)</summary>
        Task<IList<DaoUser>> GetUsers();
#endif

        /// <summary>Sends the forgotten password email to the user</summary>
        /// <exception cref="DatabaseException">["token_not_saved"] DEBUG ONLY</exception>
        /// <exception cref="ResourceNotFoundException">["user"] DEBUG ONLY</exception>
        Task SendForgotPasswordMail(string email, DaoLangTypes.Type lang);

        /// <summary>Updates the password of the user</summary>
        /// <exception cref="DatabaseException">["password_not_saved", "token_deletion_failed"]</exception>
        /// <exception cref="ResourceGoneException">["token_invalid_or_expired"]</exception>
        /// <exception cref="ResourceNotFoundException">["user"]</exception>
        Task UpdatePassword(PasswordUpdate passwordUpdate);

        /// <summary>Updates the user's language to a new one</summary>
        /// <exception cref="DatabaseException">["lang_update_failed"]</exception>
        /// <returns></returns>
        Task UpdateLang(long userId, SetLang language);

        /// <summary>Updates the bank account number of the user</summary>
        /// <exception cref="DatabaseException">["account_number_update_failed"]</exception>
        /// <exception cref="ResourceNotFoundException">["user"]</exception>
        Task UpdateBankAccoutNumber(long userId, String accountNumber);
	}
}
