using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MShare_ASP.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class HistoryService : IHistoryService
    {
        private MshareDbContext Context { get; }
        public HistoryService(MshareDbContext context)
        {
            Context = context;
        }
        public async Task<IList<DaoHistory>> GetGroupHistory(long userId, long groupId)
        {
            var daoGroup = await Context.Groups
                                .Include(x => x.Members).ThenInclude(x => x.User)
                                .SingleOrDefaultAsync(x => x.Id == groupId);

            var spendings = await Context.Spendings
                .Where(x => x.GroupId == groupId)
                .ToListAsync();
            var members = daoGroup.Members;

            return await Context.History
                .Where(
                    history => history.AffectedId == daoGroup.Id // Affecting group settings itself
                            || members.Any(member => member.UserId == history.AffectedId) // Affecting any member data?
                            || spendings.Any(spending => spending.Id == history.AffectedId)) // Affecting spending data itself
                .ToListAsync();
        }

        public async Task LogHistory(long userId, long affectedId, DaoLogType.Type type, DaoLogSubType.Type subType, object data)
        {
            var serializedMessage = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings()
            {
                MaxDepth = 4
            });

            var historyEntity = new DaoHistory()
            {
                UserId = userId,
                AffectedId = affectedId,
                Date = DateTime.UtcNow,
                Type = type,
                SubType = subType,
                SerializedLog = serializedMessage
            };

            await Context.AddAsync(historyEntity);
            await Context.SaveChangesAsync();
        }
    }
}
