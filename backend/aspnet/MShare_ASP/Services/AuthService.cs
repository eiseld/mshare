using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;

namespace MShare_ASP.Services {
    internal class AuthService : IAuthService {
        const string CHAR_SET = "0123456789qwertzuioplkjhgfdsayxcvbnm-_QWERTZUIOPLKJHGFDSAYXCVBNM";

        private Configurations.IJWTConfiguration _jwtConf;
        private Configurations.IURIConfiguration _uriConf;
        private IEmailService _emailService;
        private ITimeService _timeService;
        private MshareDbContext _context;

        public AuthService(MshareDbContext context, IEmailService emailService, ITimeService timeService, Configurations.IJWTConfiguration jwtConf, Configurations.IURIConfiguration uriConf) {
            _emailService = emailService;
            _timeService = timeService;
            _context = context;
            _jwtConf = jwtConf;
            _uriConf = uriConf;
        }

        public string Login(LoginCredentials credentials) {
            string hashedPassword = Hasher.GetHash(credentials.Password);
            var usr = _context.Users
                 .Include(x => x.EmailTokens)
                .FirstOrDefault(x => x.Email == credentials.Email && x.Password == hashedPassword);

            if (usr == null)
                throw new Exceptions.ResourceForbiddenException("User");
            if (usr.EmailTokens.Any(x => x.TokenType == DaoEmailToken.Type.Validation)) {
                throw new Exceptions.BusinessException("not_verified");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConf.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usr.Id.ToString(), ClaimValueTypes.Integer64)
                }),
                Expires = _timeService.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> Register(NewUser newUser) {
            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == newUser.Email);

            if (existingUser != null)
                throw new Exceptions.BusinessException("email_taken");

            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    var emailToken = new DaoEmailToken() {
                        TokenType = DaoEmailToken.Type.Validation,
                        ExpirationDate = _timeService.UtcNow.AddDays(1),
                        Token = GenerateRandomString(40)
                    };

                    var userToBeInserted = new DaoUser() {
                        DisplayName = newUser.DisplayName,
                        Email = newUser.Email,
                        Password = Hasher.GetHash(newUser.Password),
                        EmailTokens = new DaoEmailToken[] {
                            emailToken
                        }
                    };

                    await _context.Users.AddAsync(userToBeInserted);

                    if (await _context.SaveChangesAsync() != 2)
                        throw new Exceptions.DatabaseException();

                    await _emailService.SendMailAsync(MimeKit.Text.TextFormat.Text, newUser.DisplayName, newUser.Email, "MShare Registration", $"Your registration was successful, please proceed to the activation link... {emailToken.Token}");

                    transaction.Commit();
                    return true;
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<bool> Validate(string token) {
            var emailToken = await _context.EmailTokens.FindAsync(token);

            if (emailToken == null)
                throw new Exceptions.ResourceGoneException();
            if (emailToken.ExpirationDate > _timeService.UtcNow)
                throw new Exceptions.BusinessException("token_expired");

            _context.EmailTokens.Remove(emailToken);

            if (await _context.SaveChangesAsync() != 1) {
                throw new Exceptions.DatabaseException();
            }

            return true;
        }

        private string GenerateRandomString(int len) {
            Random r = new Random();

            char[] chars = new char[len];
            for (int i = 0; i < chars.Length; i++) {
                chars[i] = CHAR_SET[r.Next(0, CHAR_SET.Length)];
            }
            return new String(chars);
        }
    }
}
