using MShare_ASP.API.Request;
using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services {
    public interface ISpendingService {
        Task<IList<DaoSpending>> GetSpendingsForGroup(long id);
        IList<API.Response.SpendingData> ToSpendingData(IList<DaoSpending> spendings);
        Task CreateNewSpending(NewSpending newSpending, long userId);
    }
}