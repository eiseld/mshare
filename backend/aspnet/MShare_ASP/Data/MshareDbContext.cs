using Microsoft.EntityFrameworkCore;
using MShare_ASP.Utils;

namespace MShare_ASP.Data {
    /// <summary>
    /// Db Context for all data in MShare
    /// </summary>
    public class MshareDbContext : DbContext {
        /// <summary>
        /// User informations
        /// </summary>
        public DbSet<DaoUser> Users { get; set; }
        /// <summary>
        /// Group specific informations
        /// </summary>
        public DbSet<DaoGroup> Groups { get; set; }
        /// <summary>
        /// Tokens that have been sent to users
        /// </summary>
        public DbSet<DaoEmailToken> EmailTokens { get; set; }
        /// <summary>
        /// Junction table for many-to-many user-group connections
        /// </summary>
        public DbSet<DaoUsersGroupsMap> UsersGroupsMap { get; set; }

        /// <summary>
        /// Initializes a new DbContext
        /// </summary>
        /// <param name="options"></param>
        public MshareDbContext(DbContextOptions<MshareDbContext> options) :
            base(options) {

            this.ConfigureLogging(s => {
                System.Console.WriteLine(s);
            }, LoggingCategories.All);

        }

        /// <summary>
        /// Fluid rules
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DaoUsersGroupsMap>()
                .HasKey(o => new { o.UserId, o.GroupId });
				
			modelBuilder.Entity<DaoEmailToken>()
				.HasKey(o => new { o.UserId, o.Token });
        }
    }
}
