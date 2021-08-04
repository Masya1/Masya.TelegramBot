using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Masya.TelegramBot.Commands.Abstractions;
using Masya.TelegramBot.Commands.Options;
using Masya.TelegramBot.Commands.Attributes;
using Masya.TelegramBot.Commands.Metadata;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace Masya.TelegramBot.Commands.Services
{
    public class DefaultCommandService : ICommandService
    {
        protected readonly List<CommandInfo> commands;
        protected readonly IServiceProvider services;

        private readonly ILogger<DefaultCommandService> _logger;

        public IBotService BotService { get; }
        public CommandServiceOptions Options { get; }
        public List<CommandInfo> Commands => commands;

        public DefaultCommandService(IOptionsMonitor<CommandServiceOptions> options, IBotService botService, IServiceProvider services, ILogger<DefaultCommandService> logger)
        {
            BotService = botService;
            Options = options.CurrentValue;
            commands = new List<CommandInfo>();
            this.services = services;
            _logger = logger;
        }

        public virtual async Task ExecuteCommandAsync(Message message)
        {
            if (message.Contact != null)
            {
                await HandleContact(message);
                return;
            }

            if (string.IsNullOrEmpty(message.Text))
            {
                return;
            }

            await Task.Run(() => {
                var parts = new CommandParts(message.Text, Options);
                MethodInfo method = commands
                    .FirstOrDefault(cm => CommandFilter(cm.MethodInfo, parts.Name))
                    ?.MethodInfo;

                if (method == null)
                {
                    return;
                }

                if (parts.ArgsStr.Length == 0 && method.GetParameters().Length != 0)
                {
                    var t = new Task(async () => await ExecuteCommandByStepsAsync(message, method, parts));
                    t.Start();
                }
                else
                {
                    using var scope = services.CreateScope();
                    var moduleInstance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, method.DeclaringType, Array.Empty<object>());
                    var propInfo = method.DeclaringType.GetProperty("Context");
                    var context = new DefaultCommandContext(BotService, message.Chat, message.From, message);
                    propInfo.SetValue(moduleInstance, context);
                    method.Invoke(moduleInstance, parts.MatchParamTypes(method));
                }
            });
        }

        public virtual async Task LoadCommandsAsync(Assembly assembly)
        {
            _logger.LogInformation("Loading commands from modules from assembly: " + assembly.GetName().Name);
            await Task.Run(() =>
            {
                foreach (var type in assembly.DefinedTypes)
                {
                    if (IsModuleType(type))
                    {
                        foreach (var method in type.DeclaredMethods)
                        {
                            if (IsValidCommand(method))
                            {
                                commands.Add(BuildCommandInfo(method));
                            }
                        }
                    }
                }
            });
            _logger.LogInformation($"Loaded {commands.Count} commands from assembly: " + assembly.GetName().Name);
        }

        public static CommandInfo BuildCommandInfo(MethodInfo methodInfo)
        {
            string name = methodInfo
                .GetCustomAttribute<CommandAttribute>()
                ?.Name;

            string description = methodInfo
                .GetCustomAttribute<DescriptionAttribute>()
                ?.Description;

            return new CommandInfo(name, description, null, methodInfo);
        }

        public static bool IsValidCommand(MethodInfo method)
        {
            return (method.GetCustomAttribute<CommandAttribute>() != null || method.GetCustomAttribute<RegisterUserAttribute>() != null) &&
                method.IsPublic &&
                !method.IsAbstract &&
                !method.IsGenericMethod &&
                (method.ReturnType == typeof(Task) || method.ReturnType == typeof(Task<>));
        }

        public static bool IsModuleType(TypeInfo type)
        {
            return type.IsPublic &&
                !type.IsAbstract &&
                !type.IsGenericType &&
                type.BaseType.Equals(typeof(Module));
        }

        private static bool CommandFilter(MethodInfo info, string commandName)
        {
            CommandAttribute cmdAttr = info.GetCustomAttribute<CommandAttribute>();
            AliasAttribute aliasAttr = info.GetCustomAttribute<AliasAttribute>();
            return (cmdAttr != null && cmdAttr.Name.ToLower().Equals(commandName.ToLower())) ||
                (aliasAttr != null && aliasAttr.Aliases.Any(a => a.ToLower().Equals(commandName.ToLower())));
        }

        protected virtual Task ExecuteCommandByStepsAsync(Message message, MethodInfo method, CommandParts parts, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();
            var moduleInstance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, method.DeclaringType, Array.Empty<object>());
            var propInfo = method.DeclaringType.GetProperty("Context");
            var context = new DefaultCommandContext(BotService, message.Chat, message.From, message);
            propInfo.SetValue(moduleInstance, context);

            var result = new List<object>();

            // Parameters index
            int i = 0;
            ParameterInfo[] parameters = method.GetParameters();

            var messageCollector = BotService.CreateMessageCollector(message.Chat, TimeSpan.FromSeconds(Options.StepCommandTimeout));
            messageCollector.Collect(m => m.Text);
            messageCollector.OnStart += (sender, args) =>
            {
                SendParamMessage(parameters, i, message.Chat.Id, cancellationToken).Wait();
            };

            messageCollector.OnMessageTimeout += (sender, args) =>
            {
                BotService.Client
                .SendTextMessageAsync(
                    message.Chat,
                    "Время ожидания истекло. Введите команду ещё раз.",
                    cancellationToken: cancellationToken
                    )
                .Wait();
            };

            messageCollector.OnMessageReceived += (sender, e) =>
            {
                try
                {
                    result.Add(parts.MatchTypeParam(parameters[i], e.Message.Text));
                    i++;
                    if(i < parameters.Length)
                    {
                        SendParamMessage(parameters, i, e.Message.Chat.Id, cancellationToken).Wait();
                        return;
                    }
                    messageCollector.Finish();
                }
                catch
                {
                    SendParamMessage(parameters, i, e.Message.Chat.Id, cancellationToken).Wait();
                }
            };

            messageCollector.OnFinish += (sender, args) =>
            {
                method.Invoke(moduleInstance, result.ToArray());
            };

            messageCollector.Start();
            return Task.CompletedTask;
        }

        protected virtual async Task SendParamMessage(ParameterInfo[] parameters, int index, long chatId, CancellationToken cancellationToken)
        {
            var nameAttr = parameters[index].GetCustomAttribute<ParamNameAttribute>();
            string paramName = nameAttr?.Name ?? parameters[index].Name;
            await BotService.Client.SendTextMessageAsync(
                chatId,
                $"Пожалуйста, укажите {paramName}",
                cancellationToken: cancellationToken
                );
        }
    
        protected virtual Task HandleContact(Message message)
        {
            var handleMethod = commands
                .FirstOrDefault(c => IsRegisterUserMethod(c.MethodInfo))
                ?.MethodInfo;
            if (handleMethod == null) return Task.CompletedTask;

            using var scope = services.CreateScope();
            var moduleInstance = ActivatorUtilities.CreateInstance(scope.ServiceProvider, handleMethod.DeclaringType, Array.Empty<object>());
            var propInfo = handleMethod.DeclaringType.GetProperty("Context");
            var context = new DefaultCommandContext(BotService, message.Chat, message.From, message);
            propInfo.SetValue(moduleInstance, context);

            handleMethod.Invoke(moduleInstance, new[] { message.Contact });
            return Task.CompletedTask;
        }

        private static bool IsRegisterUserMethod(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return method.GetCustomAttribute<RegisterUserAttribute>() != null &&
                parameters.Length == 1 && parameters[0].ParameterType == typeof(Contact);
        }
    }
}