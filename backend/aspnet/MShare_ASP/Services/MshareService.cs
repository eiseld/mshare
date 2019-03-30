using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.Data;

namespace MShare_ASP.Services {
    internal class MshareService : IMshareService {
        public IEnumerable<DaoUser> Users => _context.Users
            .Include(x => x.EmailTokens);

        private readonly MshareDbContext _context;

        public MshareService(MshareDbContext context) {
            _context = context;
        }

        public async Task<DaoUser> GetUser(long id) {
            return await _context.Users.FindAsync(id);
        }
        public async Task<IList<DaoUser>> GetUsers() {
            return await _context.Users.Include(x => x.EmailTokens).ToListAsync();
        }
    }
}
