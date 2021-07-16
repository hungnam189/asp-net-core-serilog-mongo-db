using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using IdentityServices.Jwt;
using IdentityServices.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SerilogMongoDb.Database;
using SerilogMongoDb.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SerilogMongoDb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            Configuration.Bind("JwtSettings", jwtSettings);

            services.AddSingleton(jwtSettings);
            services.AddTransient<JwtTokenCreator>();

            // add Jwt Authentication
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                //Set default Authentication Schema as Bearer
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters =
                       new TokenValidationParameters
                       {
                           ValidateIssuerSigningKey = true,
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ClockSkew = TimeSpan.Zero, // remove delay of token when expire
                           ValidIssuer = jwtSettings.Issuer,
                           ValidAudience = jwtSettings.Audience,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                       };
            }).AddIdentityCookies(_ => { });

            // "mongodb://namth_identity:namth123@localhost:27017/Identity"
            // "var mongoDbContext = new MongoDbContext(Configuration.GetSection("ConnectionStrings:MongoDbIdentity").Value);"
            var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration
            {
                MongoDbSettings = new MongoDbSettings
                {
                    ConnectionString = Configuration.GetSection("MongoDbIdentity").GetSection("ConnectionString").Value,
                    DatabaseName = Configuration.GetSection("MongoDbIdentity").GetSection("Database").Value
                },
                IdentityOptionsAction = options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                    options.Lockout.MaxFailedAccessAttempts = 10;

                    // ApplicationUser settings
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@.-_";
                }
            };

            // "services.AddIdentity<ApplicationUser, ApplicationRole>()"
            // ".AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(mongoDbContext)"
            services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbIdentityConfiguration)
                    .AddSignInManager()
                    .AddDefaultTokenProviders();

            var employeeSettings = new ExtMongoDbSettings();
            Configuration.Bind("DemoAspNetCoreMongoDb", employeeSettings);
            services.AddSingleton(employeeSettings);

            /// services.AddSingleton(typeof(IMongoDbRepository<>), typeof(MongoDbRepository<>));
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<ICompanyRepository, CompanyRepository>();

            services.AddLogging();
            services.AddControllers();
            services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "SerilogMongoDb", Version = "v1" }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SerilogMongoDb v1"));
            }
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
