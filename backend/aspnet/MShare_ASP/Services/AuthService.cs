using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MShare_ASP.API.Request;
using MShare_ASP.Configurations;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using MShare_ASP.Utils;

namespace MShare_ASP.Services
{
    internal class AuthService : IAuthService
    {
        private IJWTConfiguration JwtConf { get; }
        private IURIConfiguration UriConf { get; }
        private IEmailService EmailService { get; }
        private ITimeService TimeService { get; }
        private MshareDbContext Context { get; }

        private readonly Random random;

        public AuthService(MshareDbContext context, IEmailService emailService, ITimeService timeService, IJWTConfiguration jwtConf, IURIConfiguration uriConf)
        {
            Context = context;
            EmailService = emailService;
            TimeService = timeService;
            JwtConf = jwtConf;
            UriConf = uriConf;
            random = new Random();
        }

        public string Login(LoginCredentials credentials)
        {
            string hashedPassword = Hasher.GetHash(credentials.Password);
            var usr = Context.Users
                .Include(x => x.EmailTokens)
                .FirstOrDefault(x => x.Email == credentials.Email && x.Password == hashedPassword);

            if (usr == null)
                throw new ResourceForbiddenException("invalid_credentials");

            if (usr.EmailTokens.Any(x => x.TokenType == DaoEmailToken.Type.Validation))
                throw new BusinessException("not_verified");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtConf.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString(), ClaimValueTypes.Integer64)
                }),
                Expires = TimeService.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task Register(NewUser newUser)
        {
            var existingUser = await Context.Users
                .FirstOrDefaultAsync(x => x.Email == newUser.Email);

            if (existingUser != null)
                throw new BusinessException("email_taken");

            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var emailToken = new DaoEmailToken()
                    {
                        TokenType = DaoEmailToken.Type.Validation,
                        ExpirationDate = TimeService.UtcNow.AddSeconds(10),
                        Token = random.RandomString(40)
                    };

                    var userToBeInserted = new DaoUser()
                    {
                        DisplayName = newUser.DisplayName,
                        Email = newUser.Email,
                        Password = Hasher.GetHash(newUser.Password),
                        EmailTokens = new DaoEmailToken[] {
                            emailToken
                        }
                    };

                    await Context.Users.AddAsync(userToBeInserted);

                    if (await Context.SaveChangesAsync() != 2)
                        throw new DatabaseException("registration_not_saved");

                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Text, newUser.DisplayName, newUser.Email, "MShare Regisztráció", $"Sikeres regisztráció, az email cím megerősítéséhez kérem kattintson ide: {UriConf.URIForEndUsers}/account/confirm/{emailToken.Token}");

                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task ValidateRegistration(string token)
        {
            var emailToken = await Context.EmailTokens
                .SingleOrDefaultAsync(x => x.Token == token &&
                    x.TokenType == DaoEmailToken.Type.Validation);

            if (emailToken == null || emailToken.ExpirationDate < TimeService.UtcNow)
                throw new ResourceGoneException("token_invalid_or_expired");

            Context.EmailTokens.Remove(emailToken);

            if (await Context.SaveChangesAsync() != 1)
                throw new DatabaseException("validation_email_remove_failed");
        }
    }
}
