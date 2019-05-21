using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;

namespace MShare_ASP.Services{
    internal class UserService: IUserService{

        private readonly MshareDbContext _context;
        private Configurations.IURIConfiguration _uriConf;
        private readonly IEmailService _emailService;
        private ITimeService _timeService;
        private Random _random;


        public UserService(MshareDbContext context, ITimeService timeService, IEmailService emailService, Configurations.IURIConfiguration uriConf){
            _context = context;
            _timeService = timeService;
            _emailService = emailService;
            _uriConf = uriConf;
            _random = new Random();
        }

        public API.Response.UserData ToUserData(DaoUser daoUser) {
            return new API.Response.UserData(){
                Id = daoUser.Id,
                Name = daoUser.DisplayName
            };
        }

        public IList<API.Response.UserData> ToUserData(IList<DaoUser> daoUsers) {
            return daoUsers.Select(daoUser => ToUserData(daoUser)).ToList();
        }

        public async Task<DaoUser> GetUser(long user){
            return await _context.Users
                .SingleAsync(x => x.Id == user);
        }

        public async Task<IList<DaoUser>> GetUsers(){
            return await _context.Users
                .Include(x => x.EmailTokens)
                .Include(x => x.Groups).ThenInclude(x => x.User)
                .Include(x => x.Groups).ThenInclude(x => x.Group).ToListAsync();
        }

        public async Task SendForgotPasswordMail(API.Request.ValidEmail email){
            var user = await _context.Users.
                Include(x => x.EmailTokens).
                FirstOrDefaultAsync(x => 
                    x.Email == email.Email && 
                    !x.EmailTokens.Any(y => y.TokenType == DaoEmailToken.Type.Validation));

            if (user != null){
                using (var transaction = _context.Database.BeginTransaction()){
                    try{
                        var emailToken = new DaoEmailToken(){
                            TokenType = DaoEmailToken.Type.Password,
                            ExpirationDate = _timeService.UtcNow.AddDays(1),
                            Token = _random.RandomString(40),
                            User = user
                        };

                        await _context.EmailTokens.AddAsync(emailToken);

                        if (await _context.SaveChangesAsync() != 1)
                            throw new Exceptions.DatabaseException("token_not_saved");

                        await _emailService.SendMailAsync(MimeKit.Text.TextFormat.Text, user.DisplayName, email.Email, "Elfelejtett jelszó", $"Jelszó megváltoztatásához kattintson ide: {_uriConf.URIForEndUsers}/reset?token={emailToken.Token}");
                        transaction.Commit();
                    }
                    catch{
                        transaction.Rollback();
                        // Eat all exceptions, User cannot know if this was successfull only for debug
#if DEBUG
                        throw;
#endif
                    }
                }
            }
            else{
                // Don't handle, user can't know failure
#if DEBUG
                throw new Exceptions.ResourceNotFoundException("user");
#endif
            }
        }

        public async Task UpdatePassword(PasswordUpdate passwordUpdate){
            var user = await _context.Users
                .Include(x => x.EmailTokens)
                .FirstOrDefaultAsync(x => x.Email == passwordUpdate.Email);

            if (user == null)
                throw new Exceptions.ResourceNotFoundException("user");

            var emailToken = user.EmailTokens.FirstOrDefault(y =>
                                y.Token == passwordUpdate.Token &&
                                y.TokenType == DaoEmailToken.Type.Password);

            if (emailToken == null)
                throw new Exceptions.ResourceGoneException("token");

            if (emailToken.ExpirationDate < _timeService.UtcNow)
                    throw new Exceptions.BusinessException("token_expired");

            using (var transaction = _context.Database.BeginTransaction()){
                try{
                    var previousPassword = user.Password;
                    user.Password = Hasher.GetHash(passwordUpdate.Password);

                    if (previousPassword != user.Password && await _context.SaveChangesAsync() != 1)
                        throw new Exceptions.DatabaseException("password_not_saved");

                    _context.EmailTokens.Remove(emailToken);
                    if (await _context.SaveChangesAsync() != 1)
                        throw new Exceptions.DatabaseException("token_deletion_failed");

                    await _emailService.SendMailAsync(MimeKit.Text.TextFormat.Text, user.DisplayName, user.Email, "Jelszó változtatás", $"A mai napon jelszava megváltoztatásra került!");

                    transaction.Commit();
                }
                catch{
                    transaction.Rollback();
                    throw;
                }
            }

        }

    }
}
