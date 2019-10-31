using EmailTemplates;
using EmailTemplates.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using MShare_ASP.API.Request;
using MShare_ASP.Configurations;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using MShare_ASP.Utils;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MShare_ASP.Services
{
    internal class AuthService : IAuthService
    {
        private IJWTConfiguration JwtConf { get; }
        private IURIConfiguration UriConf { get; }
        private IEmailService EmailService { get; }
        private ITimeService TimeService { get; }
        private MshareDbContext Context { get; }
        private IRazorViewToStringRenderer Renderer { get; }
        private IStringLocalizer<LocalizationResource> Localizer { get; }

        private readonly Random random;

        public AuthService(MshareDbContext context, IEmailService emailService, ITimeService timeService, IJWTConfiguration jwtConf, IURIConfiguration uriConf, IStringLocalizer<LocalizationResource> localizer, IRazorViewToStringRenderer renderer)
        {
            Context = context;
            EmailService = emailService;
            TimeService = timeService;
            JwtConf = jwtConf;
            UriConf = uriConf;
            Renderer = renderer;
            Localizer = localizer;
            random = new Random();
        }

        public async Task<string> Login(LoginCredentials credentials)
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

            usr.Lang = credentials.Lang;
            await Context.SaveChangesAsync();

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
                        ExpirationDate = TimeService.UtcNow.AddDays(1),
                        Token = random.RandomString(40)
                    };

                    var userToBeInserted = new DaoUser()
                    {
                        DisplayName = newUser.DisplayName,
                        Email = newUser.Email,
                        Password = Hasher.GetHash(newUser.Password),
                        EmailTokens = new DaoEmailToken[] {
                            emailToken
                        },
                        Lang = newUser.Lang
                    };

                    await Context.Users.AddAsync(userToBeInserted);

                    if (await Context.SaveChangesAsync() != 2)
                        throw new DatabaseException("registration_not_saved");

                    var model = new ConfirmationViewModel()
                    {
                        Title = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_SUBJECT),
                        PreHeader = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_PREHEADER),
                        Hero = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_HERO),
                        Greeting = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_CASUAL_BODY_GREETING, newUser.DisplayName),
                        Intro = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_BODY_INTRO),
                        EmailDisclaimer = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_BODY_DISCLAIMER),
                        Cheers = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_CASUAL_BODY_CHEERS),
                        BadButton = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_FOOTER_BADBUTTON),
                        MShareTeam = Localizer.GetString(newUser.Lang, LocalizationResource.MSHARE_TEAM),
                        SiteBaseUrl = $"{UriConf.URIForEndUsers}",
                        Button = new EmailButtonViewModel()
                        {
                            Url = $"{UriConf.AndroidOpener}confirmRegistration/{emailToken.Token}",
                            Text = Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_BODY_BUTTON)
                        }
                    };
                    var htmlBody = await Renderer.RenderViewToStringAsync($"/Views/Emails/Confirmation/ConfirmationHtml.cshtml", model);
                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Html, newUser.DisplayName, newUser.Email, Localizer.GetString(newUser.Lang, LocalizationResource.EMAIL_REGISTER_SUBJECT), htmlBody);

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