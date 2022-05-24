using Application.UseCases.Base;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Discord.Commands;
using Domain.Results;
using Infrastructure.Discord.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class Unsubscribe : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<Unsubscribe> _logger;

    public Unsubscribe(ICommandDispatcher commandDispatcher, ILogger<Unsubscribe> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }
    
    [Command("unsubscribe")]
    [Alias("us")]
    public async Task UnsubscribeHandler(string mediaName)
    {
        var unsubscribeResult =
            await _commandDispatcher.Dispatch<UnsubscribeMediaCommand, Result>(
                new UnsubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (unsubscribeResult.IsFailure)
        {
            _logger.LogInformation(unsubscribeResult.Error.ToString());
            message = "Unsubscribe failed.";
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }
}