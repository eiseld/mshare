using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MShare_ASP.Data;

namespace MShare_ASP.Services {
    public class MshareService : IMshareService {
        public IEnumerable<Test> Test => _context.test;

        private readonly MshareDbContext _context;

        public MshareService(MshareDbContext context) {
            _context = context;
        }
    }
}
