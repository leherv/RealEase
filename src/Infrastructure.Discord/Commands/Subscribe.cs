using Application.UseCases.Base;
using Application.UseCases.Subscriber.SubscribeMedia;
using Discord.Commands;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Discord.Extensions;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class Subscribe : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<Subscribe> _logger;

    internal Subscribe(ICommandDispatcher commandDispatcher, ILogger<Subscribe> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Command("subscribe")]
    [Alias("s")]
    internal async Task SubscribeHandler(string mediaName)
    {
        var subscribeResult =
            await _commandDispatcher.Dispatch<SubscribeMediaCommand, Result>(
                new SubscribeMediaCommand(Context.GetUserId().ToString(), mediaName));

        var message = "Done.";
        if (subscribeResult.IsFailure)
        {
            _logger.LogError(subscribeResult.Error.ToString());
            message = BuildErrorMessage(subscribeResult);
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }

    private static string BuildErrorMessage(Result result)
    {
        const string message = "Subscribe failed: ";
        return message + result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            _ => "Something went wrong"
        };
    }
}