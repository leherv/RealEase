using Application.UseCases.Base;
using Application.UseCases.Subscriber.UnsubscribeMedia;
using Discord.Commands;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Discord.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class Unsubscribe : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<Unsubscribe> _logger;

    internal Unsubscribe(ICommandDispatcher commandDispatcher, ILogger<Unsubscribe> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }
    
    [Command("unsubscribe")]
    [Alias("us")]
    internal async Task UnsubscribeHandler(string mediaName)
    {
        var unsubscribeResult =
            await _commandDispatcher.Dispatch<UnsubscribeMediaCommand, Result>(
                new UnsubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (unsubscribeResult.IsFailure)
        {
            _logger.LogError(unsubscribeResult.Error.ToString());
            message = BuildErrorMessage(unsubscribeResult);
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }
    
    private static string BuildErrorMessage(Result result)
    {
        const string message = "Unsubscribing failed: ";
        return message + result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Subscriber.UnsubscribeFailedErrorCode => "Subscription not found",
            _ => "Something went wrong"
        };
    }
}