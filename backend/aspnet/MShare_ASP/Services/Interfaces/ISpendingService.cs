using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    /// <summary>Spending related services</summary>
    public interface ISpendingService
    {
        /// <summary>Converts spending data from internal to facing</summary>
        SpendingData ToSpendingData(DaoSpending daoSpending);

        /// <summary>Converts spending datas from internal to facing</summary>
        IList<SpendingData> ToSpendingData(IList<DaoSpending> daoSpendings);

        /// <summary>Converts optimizedDebt data from internal to facing</summary>
        OptimisedDebtData ToOptimisedDebtData(DaoOptimizedDebt daoOptimizedDebt);

        /// <summary>Converts optimizedDebt datas from internal to facing</summary>
        IList<OptimisedDebtData> ToOptimisedDebtData(IList<DaoOptimizedDebt> daoOptimizedDebts);

        /// <summary>Gets all spending associated with a group</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        Task<IList<DaoSpending>> GetSpendingsForGroup(long userId, long groupId);

        /// <summary>Creates a new spending in the database</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        Task<IList<DaoOptimizedDebt>> GetOptimizedDebtForGroup(long userId, long groupId);

        /// <summary>Creates a new spending in the database</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member"]</exception>
        /// <exception cref="BusinessException">["debtor_not_member"]</exception>
        /// <exception cref="DatabaseException">["spending_not_inserted"]</exception>
        Task CreateNewSpending(long userId, NewSpending newSpending);

        /// <summary>Updates an existing spending in the database</summary>
        /// <exception cref="ResourceNotFoundException">["group"]</exception>
        /// <exception cref="ResourceForbiddenException">["not_group_member", "not_creditor"]</exception>
        /// <exception cref="BusinessException">["debtor_not_member"]</exception>
        /// <exception cref="ResourceGoneException">["spending"]</exception>
        /// <exception cref="DatabaseException">["spending_not_updated"]</exception>
        Task UpdateSpending(long userId, SpendingUpdate spendingUpdate);

		/// <summary>Delete a spending from the database</summary>
		/// <exception cref="ResourceNotFoundException">["group"]</exception>
		/// <exception cref="ResourceForbiddenException">["not_group_member", "not_creditor"]</exception>
		/// <exception cref="BusinessException">["debtor_not_member"]</exception>
		/// <exception cref="ResourceGoneException">["spending"]</exception>
		/// <exception cref="DatabaseException">["spending_not_deleted"]</exception>
		Task DeleteSpending(long userId, long spendingId, long groupId);

		/// <summary>Settles a debt</summary>
		/// <exception cref="ResourceNotFoundException">["group"]</exception>
		/// <exception cref="ResourceForbiddenException">["not_group_member", "lender_not_member"]</exception>
		/// <exception cref="ResourceGoneException">["debt"]</exception>
		/// <exception cref="BusinessException">["debt_already_payed"]</exception>
		/// <exception cref="DatabaseException">["debt_not_settled"]</exception>
		Task DebtSettlement(long userId, long debtorId, long lenderId, long groupId);
    }
}