using MShare_ASP.Data;
using MShare_ASP.Utils;
using System;

namespace MShare_ASP_Tests.Boilerplate
{
    public static class DbContextExtensions
    {
        public static void Seed(this MshareDbContext dbContext)
        {
            dbContext.Users.Add(new DaoUser()
            {
                Id = 0,
                CreationDate = DateTime.Parse("2019-03-30"),
                DisplayName = "Test",
                Email = "test@test.hu",
                Password = Hasher.GetHash("Default0")
            });
            dbContext.SaveChanges();
        }
    }
}