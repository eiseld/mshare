using Microsoft.EntityFrameworkCore;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class LoggingService : ILoggingService
    {
        private MshareDbContext Context { get; }

        public LoggingService(MshareDbContext context)
        {
            Context = context;
        }

        public async Task LogForGroup(long userId, long groupId, object data)
        {
            var group = Context.Groups.Include(x => x.Members).SingleOrDefault(x => x.Id == groupId);

            if (group == null)
                throw new ResourceGoneException("group_gone");

            if (!group.Members.Any(x => x.UserId == userId))
                throw new ResourceForbiddenException("user_not_member");

            var serializedMessage = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                MaxDepth = 10
            });

            await Context.History.AddAsync(new DaoHistory()
            {
                UserId = userId,
                GroupId = groupId,
                SerializedLog = serializedMessage,
                Date = DateTime.UtcNow
            });

            Console.WriteLine(serializedMessage);

            if (await Context.SaveChangesAsync() != 1)
                throw new DatabaseException("log_not_saved");
        }
    }
}
