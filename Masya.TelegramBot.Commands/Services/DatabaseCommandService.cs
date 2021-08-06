﻿using Masya.TelegramBot.Commands.Abstractions;
using Masya.TelegramBot.Commands.Data;
using Masya.TelegramBot.Commands.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Masya.TelegramBot.Commands.Services
{
    public class DatabaseCommandService : DefaultCommandService
    {
        public DatabaseCommandService(
            CommandDbContext context,
            IOptionsMonitor<CommandServiceOptions> options,
            IBotService botService,
            IServiceProvider services,
            ILogger<DefaultCommandService> logger
            )
            : base(options, botService, services, logger) { }

        public override async Task LoadCommandsAsync(Assembly assembly)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            await base.LoadCommandsAsync(assembly);
            await dbContext.AttachCommandsAsync(commands);
        }
    }
}
