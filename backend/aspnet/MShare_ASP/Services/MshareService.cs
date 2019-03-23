using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.Data;

namespace MShare_ASP.Services {
    internal class MshareService : IMshareService {
        public IEnumerable<Test> Test => _context.test;

        public IEnumerable<DaoUser> Users => _context.users
            .Include(x => x.email_tokens);

        private readonly MshareDbContext _context;

        public MshareService(MshareDbContext context) {
            _context = context;
        }

        public DaoUser GetUser(long id) {
            return _context.users.Find(id);
        }
    }
}
