using Application.UseCases.Base;
using Application.UseCases.Subscriber.SubscribeMedia;
using Discord.Commands;
using Domain.Results;
using Infrastructure.Discord.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class Subscribe : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<PublicModule> _logger;

    public Subscribe(ICommandDispatcher commandDispatcher, ILogger<PublicModule> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Command("subscribe")]
    [Alias("s")]
    public async Task SubscribeHandler(string mediaName)
    {
        var subscribeResult =
            await _commandDispatcher.Dispatch<SubscribeMediaCommand, Result>(
                new SubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (subscribeResult.IsFailure)
        {
            _logger.LogInformation(subscribeResult.Error.ToString());
            message = "Subscribe failed.";
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }
}