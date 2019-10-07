﻿using System.Collections.Generic;
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
        Task SendForgotPasswordMail(ValidEmail email);

        /// <summary>Updates the password of the user</summary>
        /// <exception cref="DatabaseException">["password_not_saved", "token_deletion_failed"]</exception>
        /// <exception cref="ResourceGoneException">["token_invalid_or_expired"]</exception>
        /// <exception cref="ResourceNotFoundException">["user"]</exception>
        Task UpdatePassword(PasswordUpdate passwordUpdate);

		/// <summary>Updates the bank account number of the user</summary>
		/// <exception cref="DatabaseException">["bank_account_number_not_saved"]</exception>
		/// <exception cref="ResourceNotFoundException">["user"]</exception>
		Task UpdateBankAccoutNumber(BankAccountNumberUpdate bankAccountNumberUpdate);
	}
}