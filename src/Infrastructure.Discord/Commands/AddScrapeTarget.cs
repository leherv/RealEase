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

        var message = "ScrapeTarget successfully added.";
        if (addScrapeTargetResult.IsFailure)
        {
            _logger.LogError(addScrapeTargetResult.Error.ToString());
            message = BuildErrorMessage(addScrapeTargetResult);
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }

    private static string BuildErrorMessage(Result result)
    {
        const string message = "Adding ScrapeTarget failed: ";
        return message + result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            Errors.Media.ScrapeTargetExistsErrorCode => "ScrapeTarget already exists",
            Errors.Scraper.ScrapeFailedErrorCode => "Scraping for media failed. Check if you inserted the relative path correctly. If it is correct it is likely this is due to a problem with the target site. Please try again later.",
            Errors.Scraper.ScrapeMediaNameFailedErrorCode => "Scraping for media name failed. Check if you inserted the relative path correctly. If it is correct it is likely this is due to a problem with the target site. Please try again later.",
            Errors.Media.ScrapeTargetReferencesOtherMediaErrorCode => "ScrapeTarget references different media",
            _ => "Something went wrong"
        };
    }
}