using Application.UseCases.Base;
using Application.UseCases.Media.AddScrapeTarget;
using Discord.Commands;
using Domain.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class AddScrapeTarget : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<Subscribe> _logger;

    internal AddScrapeTarget(ICommandDispatcher commandDispatcher, ILogger<Subscribe> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Command("addScrapeTarget")]
    [Alias("aST")]
    internal async Task SubscribeHandler(string mediaName, string websiteName, string relativeUrl)
    {
        var addScrapeTargetResult =
            await _commandDispatcher.Dispatch<AddScrapeTargetCommand, Result>(
                new AddScrapeTargetCommand(mediaName, websiteName, relativeUrl));

        var message = "Done.";
        if (addScrapeTargetResult.IsFailure)
        {
            _logger.LogInformation(addScrapeTargetResult.Error.ToString());
            message = "Adding ScrapeTarget failed.";
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }
}