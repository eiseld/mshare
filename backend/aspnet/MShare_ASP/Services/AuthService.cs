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

        private Settings _settings;
        private MshareDbContext _context;

        public AuthService(MshareDbContext context, Settings settings) {
            _context = context;
            _settings = settings;
        }

        public string Login(LoginCredentials credentials) {
            string hashedPassword = Hasher.GetHash(credentials.Password);
            var usr = _context.users
                 .Include(x => x.email_tokens)
                .FirstOrDefault(x => x.email == credentials.Email && x.password == hashedPassword);
            if (usr == null || usr.email_tokens.Any(x => x.token_type == DaoEmailToken.Type.Validation))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.JWTSecret);
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usr.id.ToString(), ClaimValueTypes.Integer64)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> Register(NewUser newUser) {
            var existingUser = await _context.users.FirstOrDefaultAsync(x => x.email == newUser.Email);
            if (existingUser != null)
                return false;

            var userToBeInserted = new DaoUser() {
                display_name = newUser.DisplayName,
                email = newUser.Email,
                password = Hasher.GetHash(newUser.Password),
                email_tokens = new DaoEmailToken[] {
                    new DaoEmailToken() {
                        token_type = DaoEmailToken.Type.Validation,
                        expiration_date = DateTime.Now.AddDays(1),
                        token = GenerateRandomString(40)
                    }
                }
            };

            await _context.users.AddAsync(userToBeInserted);

            return await _context.SaveChangesAsync() == 2;
        }

        public async Task<bool> Validate(string token) {
            var emailToken = await _context.email_tokens.FindAsync(token);
            if (emailToken != null) {
                _context.email_tokens.Remove(emailToken);
                return await _context.SaveChangesAsync() == 1;
            } else {
                return false;
            }
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
