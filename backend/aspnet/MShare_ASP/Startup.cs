using EmailTemplates;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MShare_ASP.Data;
using MShare_ASP.Middlewares;
using MShare_ASP.Services;
using MShare_ASP.Utils;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conf = MShare_ASP.Configurations;

namespace MShare_ASP
{
    /// <summary>Startup for the servcer</summary>
    public class Startup
    {
        /// <summary>Initializes a new startup with a configuration</summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>Configuration of this server</summary>
        public IConfiguration Configuration { get; }

        /// <summary>This method gets called by the runtime. Use this method to add services to the container. </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore(options =>
                {
                    options.ReturnHttpNotAcceptable = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddApiExplorer()
                .AddJsonFormatters()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                })
                .AddAuthorization()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<API.Request.LoginCredentialsValidator>();
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                })
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(); ;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "MShare API",
                    Version = "v1",
                    Description = "API for use from Web and Android",
                });

                c.DescribeAllEnumsAsStrings();

                c.DocumentFilter<APIPrefixFilter>();

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Type = "apiKey"
                }
                );

                c.AddSecurityRequirement(security);
                var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "MShare_ASP.xml");
                c.IncludeXmlComments(filePath);

                c.CustomSchemaIds(x => x.FullName);
            });

            services.AddSingleton<Conf.IEmailConfiguration>(Configuration.GetSection("EmailConfiguration")
                .Get<Conf.EmailConfiguration>());

            services.AddSingleton<Conf.IJWTConfiguration>(Configuration.GetSection("JWTConfiguration")
                .Get<Conf.JWTConfiguration>());

            services.AddSingleton<Conf.IURIConfiguration>(Configuration.GetSection("URIConfiguration")
                .Get<Conf.URIConfiguration>());

            services.AddDbContext<MshareDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MySqlSrvConnection"))
            );

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<ITimeService, TimeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ISpendingService, SpendingService>();
            services.AddTransient<IOptimizedService, OptimizedService>();
            services.AddTransient<IHistoryService, HistoryService>();
            services.AddTransient<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedLanguages = System.Enum.GetNames(typeof(DaoLangTypes.Type));
                var supportedCultures = supportedLanguages.Select(x => new System.Globalization.CultureInfo(x)).ToArray();

                options.DefaultRequestCulture =
                new Microsoft.AspNetCore.Localization.RequestCulture(culture: DaoLangTypes.Type.EN.ToString(), uiCulture: DaoLangTypes.Type.EN.ToString());
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                /*
                 * WE PROBABLY WANT TO USE LOCALIZATION WITHCULTURE OPTION AND NOT INJECT A NEW REQUEST TO DB EVERY TIME WE NEED A STRING!!
                 */

                //options.RequestCultureProviders.Insert(0, new Microsoft.AspNetCore.Localization.CustomRequestCultureProvider(async context =>
                //{
                //    return new Microsoft.AspNetCore.Localization.ProviderCultureResult("en");
                //}));
            });

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MShareSettings")["SecretKey"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            System.Console.WriteLine(Configuration.GetSection("MShareSettings")["UrlForUsers"]);
        }

        /// <summary>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseStaticFiles();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("swagger/v1/swagger.json", "My api v1");
                c.RoutePrefix = "";
            });

            app.UseRequestLocalization();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseAuthentication();
        }
    }
}