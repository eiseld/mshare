using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Configurations;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using MShare_ASP.Utils;

namespace MShare_ASP.Services
{

    internal class UserService : IUserService
    {
        private MshareDbContext Context { get; }
        private IURIConfiguration UriConf { get; }
        private IEmailService EmailService { get; }
        private ITimeService TimeService { get; }
        private readonly Random random;

        /// <summary>Gets user with given email</summary>
        /// <exception cref="ResourceNotFoundException">["user"]</exception>
        private async Task<DaoUser> GetUser(string email)
        {
            var daoUser = await Context.Users
                .Include(user => user.EmailTokens)
                .SingleOrDefaultAsync(user => user.Email == email &&
                !user.EmailTokens.Any(token => token.TokenType == DaoEmailToken.Type.Validation));

            if (daoUser == null)
                throw new ResourceNotFoundException("user");

            return daoUser;
        }

        public UserService(MshareDbContext context, ITimeService timeService, IEmailService emailService, Configurations.IURIConfiguration uriConf)
        {
            Context = context;
            TimeService = timeService;
            EmailService = emailService;
            UriConf = uriConf;
            random = new Random();
        }

        public UserData ToUserData(DaoUser daoUser)
        {
            return new UserData()
            {
                Id = daoUser.Id,
                Name = daoUser.DisplayName
            };
        }

        public IList<UserData> ToUserData(IList<DaoUser> daoUsers)
        {
            return daoUsers.Select(daoUser => ToUserData(daoUser)).ToList();
        }

        public async Task<DaoUser> GetUser(long userId)
        {
            var daoUser = await Context.Users
                .Include(user => user.EmailTokens)
                .SingleOrDefaultAsync(user => user.Id == userId &&
                !user.EmailTokens.Any(token => token.TokenType == DaoEmailToken.Type.Validation));

            if (daoUser == null)
                throw new ResourceNotFoundException("user");

            return daoUser;
        }

#if DEBUG
        public async Task<IList<DaoUser>> GetUsers()
        {
            return await Context.Users
                .Include(x => x.EmailTokens)
                .Include(x => x.Groups).ThenInclude(x => x.User)
                .Include(x => x.Groups).ThenInclude(x => x.Group).ToListAsync();
        }
#endif

        public async Task SendForgotPasswordMail(ValidEmail email)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var daoUser = await GetUser(email.Email);

                    var emailToken = new DaoEmailToken()
                    {
                        TokenType = DaoEmailToken.Type.Password,
                        ExpirationDate = TimeService.UtcNow.AddDays(1),
                        Token = random.RandomString(40),
                        User = daoUser
                    };

                    await Context.EmailTokens.AddAsync(emailToken);

                    if (await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("token_not_saved");

                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Text, daoUser.DisplayName, email.Email, "Elfelejtett jelszó", $"Jelszó megváltoztatásához kattintson ide: {UriConf.URIForEndUsers}/reset?token={emailToken.Token}");
                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    // Eat all exceptions, User cannot know if this was successfull only for debug
#if DEBUG
                    throw;
#endif
                }
            }
        }

        public async Task UpdatePassword(PasswordUpdate passwordUpdate)
        {
            var daoUser = await GetUser(passwordUpdate.Email);

            var emailToken = daoUser.EmailTokens
                .FirstOrDefault(x =>
                                x.Token == passwordUpdate.Token &&
                                x.TokenType == DaoEmailToken.Type.Password);

            if (emailToken == null || emailToken.ExpirationDate < TimeService.UtcNow)
                throw new ResourceGoneException("token_invalid_or_expired");

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var previousPassword = daoUser.Password;
                    daoUser.Password = Hasher.GetHash(passwordUpdate.Password);

                    if (previousPassword != daoUser.Password && await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("password_not_saved");

                    Context.EmailTokens.Remove(emailToken);

                    if (await Context.SaveChangesAsync() != 1)
                        throw new DatabaseException("token_deletion_failed");

                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Text, daoUser.DisplayName, daoUser.Email, "Jelszó változtatás", $"A mai napon jelszava megváltoztatásra került!");

                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

    }
}
