using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;

namespace MShare_ASP.Services {
    internal class MshareService : IMshareService {
        public IEnumerable<DaoUser> Users => _context.Users
            .Include(x => x.EmailTokens);

        private readonly MshareDbContext _context;
        private Configurations.IURIConfiguration _uriConf;
        private readonly IEmailService _emailService;
        private ITimeService _timeService;
        private Random _random;

        public MshareService(MshareDbContext context, ITimeService timeService, IEmailService emailService, Configurations.IURIConfiguration uriConf) {
            _context = context;
            _timeService = timeService;
            _emailService = emailService;
            _uriConf = uriConf;
            _random = new Random();
        }

        public async Task<DaoUser> GetUser(long id) {
            return await _context.Users
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Group)
                        .ThenInclude(x => x.CreatorUser)
                .Include(x => x.Groups)
                    .ThenInclude(x => x.Group)
                        .ThenInclude(x => x.Members)
                            .ThenInclude(x => x.User)
                .SingleAsync(x => x.Id == id);
        }
        public async Task<IList<DaoUser>> GetUsers() {
            return await _context.Users.Include(x => x.EmailTokens)
                .Include(x => x.Groups).ThenInclude(x => x.User)
                .Include(x => x.Groups).ThenInclude(x => x.Group).ToListAsync();
        }

        public async Task<IList<DaoGroup>> GetGroups() {
            return await _context.Groups
                .Include(x => x.Members).ThenInclude(x => x.User).ToListAsync();
        }

        public async Task SendForgotPasswordMail(API.Request.ValidEmail email) {
            var user = await _context.Users.Include(x => x.EmailTokens).FirstOrDefaultAsync(x => x.Email == email.Email && !x.EmailTokens.Any(y => y.TokenType == DaoEmailToken.Type.Validation));

            if (user != null) {
                using (var transaction = _context.Database.BeginTransaction()) {
                    try {
                        var emailToken = new DaoEmailToken() {
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
                    } catch {
                        transaction.Rollback();
                        // Eat all exceptions, User cannot know if this was successfull only for debug
#if DEBUG
                        throw;
#endif
                    }
                }
            } else {
                // Don't handle, user can't know failure
#if DEBUG
                throw new Exceptions.ResourceNotFoundException("user");
#endif
            }
        }
        public async Task UpdatePassword(PasswordUpdate passwordUpdate) {
            var user = await _context.Users
                                    .Include(x => x.EmailTokens)
                                    .FirstOrDefaultAsync(x => x.Email == passwordUpdate.Email
                                        && x.EmailTokens.Any(y => y.Token == passwordUpdate.Token && y.TokenType == DaoEmailToken.Type.Password));

            if (user == null) {
                throw new Exceptions.ResourceNotFoundException("user");
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                try {

                    user.Password = Hasher.GetHash(passwordUpdate.Password);

                    if (await _context.SaveChangesAsync() != 1)
                        throw new Exceptions.DatabaseException("password_not_saved");

                    await _emailService.SendMailAsync(MimeKit.Text.TextFormat.Text, user.DisplayName, user.Email, "Elfelejtett jelszó", $"A mai napon jelszava megváltoztatásra került!");

                    transaction.Commit();
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        public async Task<DaoGroup> GetGroup(long id) {
            try {
                return await _context.Groups
                                .Include(x => x.Members).ThenInclude(x => x.User)
                                .Include(x => x.CreatorUser)
                                .SingleAsync(x => x.Id == id);
            } catch (InvalidOperationException) {
                throw new Exceptions.ResourceNotFoundException("group_not_found");
            }
        }

        public async Task CreateGroup(NewGroup newGroup, long forUser) {
            var existingGroup = await _context.Groups
                                              .FirstOrDefaultAsync(x => x.CreatorUserId == forUser
                                           && x.Name == newGroup.Name);
            if (existingGroup != null)
                throw new Exceptions.BusinessException("name_taken");

            await _context.Groups.AddAsync(new DaoGroup() {
                CreatorUserId = forUser,
                Name = newGroup.Name
            });

            if (await _context.SaveChangesAsync() != 1) {
                throw new Exceptions.DatabaseException("group_not_created");
            }
        }

    }
}
