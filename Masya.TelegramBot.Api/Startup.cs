using Masya.TelegramBot.Commands.Abstractions;
using Masya.TelegramBot.Commands.Options;
using Masya.TelegramBot.DatabaseExtensions;
using Masya.TelegramBot.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Newtonsoft.Json;
using System.Text;
using Masya.TelegramBot.Api.Options;
using Masya.TelegramBot.Api.Services;
using System;

namespace Masya.TelegramBot.Api
{
    public class Startup
    {
        private const string CorsPolicyName = "DefaultCORSPolicy";

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .CreateLogger();

            services.Configure<CommandServiceOptions>(Configuration.GetSection("Commands"));
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<XmlOptions>(Configuration.GetSection("XmlOptions"));
            services.Configure<CacheOptions>(Configuration.GetSection("Cache"));
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: CorsPolicyName,
                    builder =>
                    {
                        builder.AllowAnyHeader()
                            .AllowAnyOrigin()
                            .AllowAnyMethod();
                    }
                );
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "TelegramBot_";
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("RemoteDb"));
            });
            services.AddSingleton<IBotService, DatabaseBotService>();
            services.AddSingleton<ICommandService, DatabaseCommandService>();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.UseCamelCasing(true);
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddSingleton<IJwtService, JwtService>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JwtOptions:Issuer"],
                        ValidAudience = Configuration["JwtOptions:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtOptions:Secret"])),
                        ClockSkew = TimeSpan.FromSeconds(30),
                    };
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/", async context =>
                {
                    await context.Response.WriteAsync("<h1>Kinda homepage</h1>");
                    await context.Response.CompleteAsync();
                });
                endpoints.MapControllerRoute(
                    name: "Wilcard_or_update",
                    pattern: "{**catchAll}",
                    defaults: new { Controller = "Bot", Action = "Index" }
                );
            });
        }
    }
}
