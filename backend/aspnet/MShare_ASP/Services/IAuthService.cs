using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace MShare_ASP.Services {
    public interface IAuthService {
        string Login(API.Request.LoginCredentials credentials);
        Task<bool> Register(API.Request.NewUser newUser);
        Task<bool> Validate(String token);
    }
}
