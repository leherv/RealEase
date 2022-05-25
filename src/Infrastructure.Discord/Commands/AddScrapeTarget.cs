using Application.UseCases.Base;
using Application.UseCases.Media.AddScrapeTarget;
using Discord.Commands;
using Domain.ApplicationErrors;
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
    internal async Task AddScrapeTargetHandler(string mediaName, string websiteName, string relativeUrl)
    {
        var addScrapeTargetResult =
            await _commandDispatcher.Dispatch<AddScrapeTargetCommand, Result>(
                new AddScrapeTargetCommand(mediaName, websiteName, relativeUrl));

        var message = "Done.";
        if (addScrapeTargetResult.IsFailure)
        {
            message = "Adding ScrapeTarget failed";
            _logger.LogWarning(addScrapeTargetResult.Error.ToString());
            if (addScrapeTargetResult.Error.Code == Errors.Media.ScrapeTargetExistsErrorCode)
                message += " as this ScrapeTarget is already configured";
            message += ".";
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }
}