using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using MShare_ASP.Data;
using MShare_ASP.Services;
using MShare_ASP_Tests.Boilerplate;
using MShare_ASP_Tests.Mocks;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace MShare_ASP_Tests {
    public class MShareIntegrationTestBase
    : IClassFixture<WebApplicationFactory<MShare_ASP.Startup>> {
        private readonly WebApplicationFactory<MShare_ASP.Startup> _factory;

        public MShareIntegrationTestBase(WebApplicationFactory<MShare_ASP.Startup> factory) {
            _factory = factory.WithWebHostBuilder(config => {
                config.ConfigureServices(services => {
                    services.AddDbContext<MshareDbContext>(options => {
                        options.UseInMemoryDatabase(databaseName: "mshare");
                        options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                    });


                    // Build the service provider.
                    var sp = services.BuildServiceProvider();

                    // Create a scope to obtain a reference to the database
                    // context (ApplicationDbContext).
                    using (var scope = sp.CreateScope()) {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<MshareDbContext>();

                        // Ensure the database is deleted before creating it
                        db.Database.EnsureDeleted();

                        // Ensure the database is created.
                        db.Database.EnsureCreated();

                        // Seed the database with test data.
                        db.Seed();
                    }
                });

                config.ConfigureTestServices(services => {
                    services.AddTransient<IEmailService, MockEmailService>();
                });
            });
            //= factory;
        }

        public System.Net.Http.HttpClient CreateClient() {
            return _factory.CreateClient();
        }
    }
}