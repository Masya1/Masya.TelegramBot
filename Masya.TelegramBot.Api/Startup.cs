using Masya.TelegramBot.Commands.Abstractions;
using Masya.TelegramBot.Commands.Options;
using Masya.TelegramBot.DatabaseExtensions;
using Masya.TelegramBot.DataAccess;
using Masya.TelegramBot.Api.Options;
using Masya.TelegramBot.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System;
using Coravel;
using Masya.TelegramBot.DatabaseExtensions.Metadata;
using Masya.TelegramBot.DatabaseExtensions.Abstractions;
using Masya.TelegramBot.DatabaseExtensions.Utils;
using Masya.TelegramBot.Api.Services.Abstractions;
using Microsoft.Extensions.Logging;
using Coravel.Queuing.Interfaces;

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
            services.Configure<CommandServiceOptions>(Configuration.GetSection("Commands"));
            services.Configure<JwtOptions>(Configuration.GetSection("JwtOptions"));
            services.Configure<CacheOptions>(Configuration.GetSection("Cache"));
            services.AddScheduler();
            services.AddQueue();
            services.AddTransient<UpdateXmlImportsInvokable>();
            services.AddAutoMapper(typeof(Startup));
            services.AddCors(options =>
            {
                options.AddPolicy(
                name: CorsPolicyName,
                builder =>
                {
                    builder
                .AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod();
                }
            );
            });
            services.AddSingleton<IDatabaseLogsService, DatabaseLogsService>();
            services.AddScoped<IKeyboardGenerator, KeyboardGenerator>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "TelegramBot_";
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("RemoteDb"),
                    o =>
                    {
                        o.CommandTimeout(60);
                        o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    }
                );
            });
            services.AddSingleton<IBotService<DatabaseCommandInfo, DatabaseAliasInfo>, DatabaseBotService>();
            services.AddSingleton<ICommandService<DatabaseCommandInfo, DatabaseAliasInfo>, DatabaseCommandService>();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.UseCamelCasing(true);
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            services.AddSingleton<IJwtService, JwtService>();
            services.AddScoped<IXmlService, XmlService>();
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

            app.UseStaticFiles();

            var provider = app.ApplicationServices;
            provider.UseScheduler(scheduler =>
            {
                scheduler
                    .Schedule<UpdateXmlImportsInvokable>()
                    .Daily();
            });
            provider
                .ConfigureQueue()
                .LogQueuedTaskProgress(provider.GetService<ILogger<IQueue>>());

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
