using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class OptimizedService : IOptimizedService
    {
        private MshareDbContext Context { get; }

        private Dictionary<long, long> CalculateDeltaDebts(Dictionary<long, long> oldDebts, Dictionary<long, long> newDebts)
        {
            var deltaDebts = new Dictionary<long, long>();

            foreach (var oldDebt in oldDebts)
            {
                var key = oldDebt.Key;
                var oldValue = oldDebt.Value;
                var newValue = newDebts.ContainsKey(key) ? newDebts[key] : 0;

                deltaDebts[key] = newValue - oldValue;
            }

            foreach (var newDebt in newDebts)
            {
                var key = newDebt.Key;
                var newValue = newDebt.Value;

                if (!deltaDebts.ContainsKey(key))
                {
                    deltaDebts[key] = newValue;
                }
            }

            return deltaDebts;
        }

        internal class DebtMatrix
        {
            public class OptimizedDebt
            {
                public readonly long creditorId;
                public readonly long debtorId;
                public readonly long amount;

                public OptimizedDebt(long creditorId, long debtorId, long amount)
                {
                    this.creditorId = creditorId;
                    this.debtorId = debtorId;
                    this.amount = amount;
                }
            }

            private long[][] matrix;
            private readonly Dictionary<long, int> userIdToIndex;
            private readonly Dictionary<int, long> IndexTouserId;

            public DebtMatrix(long[] userIds)
            {
                matrix = new long[userIds.Length][];
                userIdToIndex = new Dictionary<long, int>();
                IndexTouserId = new Dictionary<int, long>();

                for (int i = 0; i < userIds.Length; i++)
                {
                    matrix[i] = new long[userIds.Length];
                    userIdToIndex.Add(userIds[i], i);
                    IndexTouserId.Add(i, userIds[i]);
                }
            }

            public void UpdateDebt(long debtorId, long creditorId, long dAmount)
            {
                matrix[userIdToIndex[debtorId]][userIdToIndex[creditorId]] += dAmount;
            }

            public void SetDebt(long debtorId, long creditorId, long amount)
            {
                matrix[userIdToIndex[debtorId]][userIdToIndex[creditorId]] = amount;
            }

            public void Optimize()
            {
                var Optimizer = new SpendingOptimizer(matrix);
                Optimizer.Optimize();
                matrix = Optimizer.GetResult();
            }

            public IList<OptimizedDebt> GetOptimizedDebts()
            {
                IList<OptimizedDebt> optimizedDebts = new List<OptimizedDebt>();

                for (int i = 0; i < matrix.Length; i++)
                {
                    for (int j = 0; j < matrix.Length; j++)
                    {
                        if (matrix[i][j] > 0)
                        {
                            optimizedDebts.Add(new OptimizedDebt(IndexTouserId[j], IndexTouserId[i], matrix[i][j]));
                        }
                    }
                }

                return optimizedDebts;
            }
        }

        private async Task SaveDebtMatrix(long groupId, DebtMatrix debtMatrix)
        {
            var oldOptimizedDebts = await Context.OptimizedDebt
                .Where(x => x.GroupId == groupId)
                .ToListAsync();

            Context.OptimizedDebt.RemoveRange(oldOptimizedDebts);

            IList<DebtMatrix.OptimizedDebt> optimizedDebts = debtMatrix.GetOptimizedDebts();

            IList<DaoOptimizedDebt> daoOptimizedDebts = optimizedDebts.Select(x =>
                new DaoOptimizedDebt()
                {
                    GroupId = groupId,
                    UserOwesId = x.debtorId,
                    UserOwedId = x.creditorId,
                    OweAmount = x.amount
                }).ToList();

            await Context.AddRangeAsync(daoOptimizedDebts);
        }

        private async Task<DebtMatrix> LoadDebtMatrix(long groupId)
        {
            var daoGroup = await Context.Groups
               .SingleOrDefaultAsync(x => x.Id == groupId);

            var userIds = daoGroup.Members.Select(x => x.UserId).ToArray();

            DebtMatrix debtMatrix = new DebtMatrix(userIds);

            var optimizedDebts = await Context.OptimizedDebt
                .Where(x => x.GroupId == groupId)
                .ToListAsync();

            foreach (var optimizedDebt in optimizedDebts)
            {
                debtMatrix.UpdateDebt(optimizedDebt.UserOwesId, optimizedDebt.UserOwedId, optimizedDebt.OweAmount);
            }

            return debtMatrix;
        }

        private async Task OptimizeHelper(long groupId)
        {
            var daoGroup = await Context.Groups
                    .Include(x => x.Members).ThenInclude(x => x.User)
                    .Include(x => x.CreatorUser)
                    .SingleOrDefaultAsync(x => x.Id == groupId);

            var daoSpendings = await Context.Spendings
               .Include(x => x.Creditor)
               .Include(x => x.Debtors).ThenInclude(x => x.Debtor)
               .Where(x => x.GroupId == groupId)
               .ToListAsync();

            var daoSettlements = Context.Settlements
                .Where(x => x.GroupId == groupId);

            var userIds = daoGroup.Members.Select(x => x.UserId).ToArray();
            DebtMatrix debtMatrix = new DebtMatrix(userIds);

            foreach (var userId in userIds)
            {
                var userSpendings = daoSpendings.Where(x => x.CreditorUserId == userId);
                foreach (var userSpending in userSpendings)
                {
                    foreach (var daoDebtor in userSpending.Debtors)
                    {
                        debtMatrix.UpdateDebt(daoDebtor.DebtorUserId, userId, daoDebtor.Debt);
                    }
                }
            }

            foreach (var daoSettlement in daoSettlements)
            {
                if (daoGroup.Members.Any(x => x.UserId == daoSettlement.From) && daoGroup.Members.Any(x => x.UserId == daoSettlement.To))
                {
                    debtMatrix.UpdateDebt(daoSettlement.To, daoSettlement.From, daoSettlement.Amount);
                }
            }

            debtMatrix.Optimize();

            await SaveDebtMatrix(groupId, debtMatrix);
        }

        public OptimizedService(MshareDbContext context)
        {
            Context = context;
        }

#if DEBUG

        public async Task OptimizeForAllGroup()
        {
            var groupIds = Context.Groups.Select(x => x.Id).ToList();

            foreach (var groupId in groupIds)
            {
                await OptimizeHelper(groupId);
            }

            await Context.SaveChangesAsync();
        }

#endif

        public async Task OptimizeForGroup(long groupId)
        {
            await OptimizeHelper(groupId);
            await Context.SaveChangesAsync();
        }

        public async Task OptimizeForNewSpending(long userId, NewSpending newSpending)
        {
            DebtMatrix debtMatrix = await LoadDebtMatrix(newSpending.GroupId);

            foreach (var debtor in newSpending.Debtors)
            {
                debtMatrix.UpdateDebt(debtor.DebtorId, userId, debtor.Debt);
            }

            debtMatrix.Optimize();
            await SaveDebtMatrix(newSpending.GroupId, debtMatrix);
        }

        public async Task OptimizeForUpdateSpending(long groupId, long creditorId, Dictionary<long, long> oldDebts, Dictionary<long, long> newDebts)
        {
            DebtMatrix debtMatrix = await LoadDebtMatrix(groupId);
            Dictionary<long, long> deltaDebts = CalculateDeltaDebts(oldDebts, newDebts);

            foreach (var deltaDebt in deltaDebts)
            {
                debtMatrix.UpdateDebt(deltaDebt.Key, creditorId, deltaDebt.Value);
            }

            debtMatrix.Optimize();
            await SaveDebtMatrix(groupId, debtMatrix);
        }

        public async Task OptimizeForSettling(long groupId, long creditorId, long debtorId)
        {
            DebtMatrix debtMatrix = await LoadDebtMatrix(groupId);
            debtMatrix.SetDebt(debtorId, creditorId, 0);
            debtMatrix.Optimize();
            await SaveDebtMatrix(groupId, debtMatrix);
        }
    }
}