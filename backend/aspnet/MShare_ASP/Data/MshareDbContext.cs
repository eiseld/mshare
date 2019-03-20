using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    public class MshareDbContext : DbContext {
        public DbSet<Test> test { get; set; }

        public MshareDbContext(DbContextOptions<MshareDbContext> options) : 
            base(options) {

        }
    }
}
