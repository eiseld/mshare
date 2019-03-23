using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MShare_ASP.Data;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.Services;
using System.Web.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Text;

namespace MShare_ASP {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services
                .AddMvcCore(options => {
                    options.ReturnHttpNotAcceptable = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddApiExplorer()
                .AddJsonFormatters()
                .AddAuthorization();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info {
                    Title = "MShare API",
                    Version = "v1",
                    Description = "API for use from Web and Android"
                });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme {
                    In = "header",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    Type = "apiKey" }
                );

                c.AddSecurityRequirement(security);
                var filePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "MShare_ASP.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddDbContext<MshareDbContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MySqlSrvConnection"))
            );

            services.AddTransient<IMshareService, MshareService>();
            services.AddTransient<IAuthService, AuthService>();

            var key = Encoding.ASCII.GetBytes(Configuration.GetSection("MShareSettings")["JWTSecret"]);
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddSingleton(new Settings() {
                JWTSecret = Configuration.GetSection("MShareSettings")["JWTSecret"],
                UrlForUsers = Configuration.GetSection("MShareSettings")["UrlForUsers"]
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My api v1");
                c.RoutePrefix = "help";
            });

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseAuthentication();
        }
    }
}
