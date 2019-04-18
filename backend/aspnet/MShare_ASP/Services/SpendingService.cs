using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;

namespace MShare_ASP.Services {
    internal class SpendingService : ISpendingService {
        private MshareDbContext DbContext { get; }
        public SpendingService(MshareDbContext dbContext) {
            DbContext = dbContext;
        }
        public async Task<IList<DaoSpending>> GetSpendingsForGroup(long id) {
            return await DbContext.Spendings
                            .Where(x => x.GroupId == id)
                            .Include(x => x.Debtors).ThenInclude(x => x.Debtor)
                            .ToListAsync();
        }

        public IList<SpendingData> ToSpendingData(IList<DaoSpending> spendings) {
            return spendings.Select(x => new SpendingData() {
                MoneyOwed = x.MoneyOwed,
                Name = x.Name,
                Debtors = x.Debtors.Select(d => new DebtorData() {
                    Id = d.DebtorUserId,
                    Debt = d.Debt.Value,
                    Name = d.Debtor.DisplayName
                }).ToList()
            }).ToList();
        }

        public async Task<DaoSpending> CreateNewSpending(NewSpending newSpending, long userId) {
            var currentUser = await DbContext.Users.FindAsync(userId);
            if (currentUser == null)
                throw new ResourceGoneException("current_user_gone");

            var currentGroup = await DbContext.Groups
                                              .Include(x => x.Members)
                                              .SingleOrDefaultAsync(x => x.Id == newSpending.GroupId);

            if (currentGroup == null)
                throw new ResourceGoneException("group_gone");

            if (newSpending.Debtors.Any(x => newSpending.Debtors.Count(d => d.DebtorId == x.DebtorId) > 1))
                throw new BusinessException("duplicate_debtor_id_found");

            if (!currentGroup.Members.Any(x => x.UserId == userId))
                throw new ResourceForbiddenException("user_not_member");

            if (newSpending.Debtors.Any()
            && !newSpending.Debtors.All(x => currentGroup.Members.Any(m => m.UserId == x.DebtorId)))
                throw new BusinessException("not_all_debtors_are_members");

            if (newSpending.Debtors.Any()
            && !newSpending.Debtors.All(x => DbContext.Users.Find(x.DebtorId) != null))
                throw new ResourceGoneException("debtor_gone");


            DaoSpending spending = new DaoSpending() {
                Name = newSpending.Name,
                MoneyOwed = newSpending.MoneySpent,
                Group = currentGroup,
                GroupId = currentGroup.Id,
                Creditor = currentUser,
                CreditorUserId = currentUser.Id
            };

            var nonSpecifiedCount = newSpending.Debtors.Any()
                ? newSpending.Debtors.Count(s => !s.Debt.HasValue)
                : currentGroup.Members.Count();

            var autoCalculatedIndividualDebt = nonSpecifiedCount == 0
                ? 0
                : (newSpending.MoneySpent - newSpending.Debtors.Sum(x => x.Debt ?? 0)) / nonSpecifiedCount;

            var debtors = newSpending.Debtors.Select(x => new DaoDebtor() {
                Spending = spending,
                DebtorUserId = x.DebtorId,
                Debt = x.Debt ?? autoCalculatedIndividualDebt
            }).ToList();

            // If there were no debtors, populate it from group members
            if (!debtors.Any()) {
                debtors = currentGroup.Members.Select(x => new DaoDebtor() {
                    Spending = spending,
                    DebtorUserId = x.UserId,
                    Debt = autoCalculatedIndividualDebt
                }).ToList();
            }

            spending.Debtors = debtors.ToList();

            var insertCount = 1 + spending.Debtors.Count;
            await DbContext.Spendings.AddAsync(spending);
            if (await DbContext.SaveChangesAsync() != insertCount) {
                throw new DatabaseException("spending_not_inserted");
            }
            return spending;
        }
    }
}
