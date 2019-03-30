using Microsoft.EntityFrameworkCore;
using MShare_ASP.Utils;

namespace MShare_ASP.Data {
    internal class MshareDbContext : DbContext {
        public DbSet<DaoUser> Users { get; set; }
        public DbSet<DaoEmailToken> EmailTokens { get; set; }

        public MshareDbContext(DbContextOptions<MshareDbContext> options) :
            base(options) {

            this.ConfigureLogging(s => {
                System.Console.WriteLine(s);
            }, LoggingCategories.All);

        }
    }
}
