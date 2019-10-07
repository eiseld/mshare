using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailTemplates;
using EmailTemplates.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
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
        private IRazorViewToStringRenderer Renderer { get; }
        private IStringLocalizer<LocalizationResource> Localizer { get; }

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

        public UserService(MshareDbContext context, ITimeService timeService, IEmailService emailService, IURIConfiguration uriConf, IStringLocalizer<LocalizationResource> localizer, IRazorViewToStringRenderer renderer)
        {
            Context = context;
            TimeService = timeService;
            EmailService = emailService;
            UriConf = uriConf;
            Renderer = renderer;
            Localizer = localizer;
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

        public async Task SendForgotPasswordMail(string email, DaoLangTypes.Type lang)
        {
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var daoUser = await GetUser(email);

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


                    var model = new ConfirmationViewModel()
                    {
                        Title = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_SUBJECT),
                        PreHeader = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_PREHEADER),
                        Hero = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_HERO),
                        Greeting = Localizer.GetString(lang, LocalizationResource.EMAIL_CASUAL_BODY_GREETING, daoUser.DisplayName),
                        Intro = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_BODY_INTRO),
                        EmailDisclaimer = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_BODY_DISCLAIMER),
                        Cheers = Localizer.GetString(lang, LocalizationResource.EMAIL_CASUAL_BODY_CHEERS),
                        BadButton = Localizer.GetString(lang, LocalizationResource.EMAIL_FOOTER_BADBUTTON),
                        MShareTeam = Localizer.GetString(lang, LocalizationResource.MSHARE_TEAM),
                        SiteBaseUrl = $"{UriConf.URIForEndUsers}",
                        Button = new EmailButtonViewModel()
                        {
                            Url = $"{UriConf.URIForEndUsers}/reset?token={emailToken.Token}",
                            Text = Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_BODY_BUTTON)
                        }
                    };
                    var htmlBody = await Renderer.RenderViewToStringAsync($"/Views/Emails/Confirmation/ConfirmationHtml.cshtml", model);

                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Html, daoUser.DisplayName, email, Localizer.GetString(lang, LocalizationResource.EMAIL_FORGOTPSW_SUBJECT), htmlBody);

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

                    var model = new InformationViewModel()
                    {
                        Title = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_SUBJECT),
                        PreHeader = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_PREHEADER),
                        Hero = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_HERO),
                        Greeting = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_CASUAL_BODY_GREETING, daoUser.DisplayName),
                        Intro = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_BODY_INTRO),
                        EmailDisclaimer = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_BODY_DISCLAIMER),
                        Cheers = Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_CASUAL_BODY_CHEERS),
                        MShareTeam = Localizer.GetString(daoUser.Lang, LocalizationResource.MSHARE_TEAM),
                        SiteBaseUrl = $"{UriConf.URIForEndUsers}"
                    };
                    var htmlBody = await Renderer.RenderViewToStringAsync($"/Views/Emails/Confirmation/InformationHtml.cshtml", model);
                    await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Html, daoUser.DisplayName, daoUser.Email, Localizer.GetString(daoUser.Lang, LocalizationResource.EMAIL_PASSWORDCHANGED_SUBJECT), htmlBody);

                    transaction.Commit();
                } catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task UpdateLang(long userId, SetLang language)
        {
            var user = await GetUser(userId);

            if (user.Lang != language.Lang)
            {
                user.Lang = language.Lang;

                if (await Context.SaveChangesAsync() != 1)
                    throw new DatabaseException("lang_update_failed");
            }
        }
    
		
		public async Task UpdateBankAccoutNumber(BankAccountNumberUpdate bankAccountNumberUpdate)
		{
			var daoUser = await GetUser(bankAccountNumberUpdate.Email);
			/*
			using (var transaction = Context.Database.BeginTransaction())
			{
				try
				{

					daoUser.BankAccountNumber = bankAccountNumberUpdate.BankAccountNumber;

					if (await Context.SaveChangesAsync() != 1)
						throw new DatabaseException("bank_account_number_not_saved");

					await EmailService.SendMailAsync(MimeKit.Text.TextFormat.Text, daoUser.DisplayName, daoUser.Email, "Bankszámlaszám változtatás", $"A mai napon fiókjához tartozó bankszámlaszáma megváltoztatásra került!");

					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
			*/
		}

	}
}
