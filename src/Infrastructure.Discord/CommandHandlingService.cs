using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure.Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord;

 public class CommandHandlingService
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly CommandService _commandService;
        private ILogger<CommandHandlingService> _logger;

        public CommandHandlingService(
            CommandService commandService,
            DiscordSocketClient discordSocketClient,
            IServiceScopeFactory scopeFactory,
            ILogger<CommandHandlingService> logger
        )
        {
            _commandService = commandService;
            _client = discordSocketClient;
            _serviceProvider = scopeFactory.CreateScope().ServiceProvider;
            _logger = logger;
            _client.MessageReceived += MessageReceivedAsync;
            _commandService.CommandExecuted += CommandExecutedAsync;
        }
        
        internal async Task InitializeAsync()
        {
            // Register modules that are internal and inherit ModuleBase<T>.
            await _commandService.AddModulesAsync(typeof(Help).Assembly, _serviceProvider);
        }
        
        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (rawMessage is not SocketUserMessage { Source: MessageSource.User } message) return;

            var argPos = 0;
            // Perform prefix check. 
            if (!message.HasStringPrefix("re!", ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            
            // we will handle the result in CommandExecutedAsync,
            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
        }

        private async Task CommandExecutedAsync(
            Optional<CommandInfo> command,
            ICommandContext context,
            IResult result
        )
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            _logger.LogError("Executing command {command} failed du to: {errorDetails}", command.Value.Name, result);
            await context.Channel.SendMessageAsync("Something went wrong.");
        }
    }