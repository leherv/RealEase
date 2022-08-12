using Application.UseCases.Base;
using Application.UseCases.Media.AddMedia;
using Discord.Commands;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class AddMedia : ModuleBase<SocketCommandContext>
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<AddMedia> _logger;

    internal AddMedia(ICommandDispatcher commandDispatcher, ILogger<AddMedia> logger)
    {
        _commandDispatcher = commandDispatcher;
        _logger = logger;
    }

    [Command("addMedia")]
    [Alias("aM")]
    internal async Task AddMediaHandler(string websiteName, string relativePath)
    {
        var addMediaCommand = new AddMediaCommand(websiteName, relativePath);
        var addMediaResult =
            await _commandDispatcher.Dispatch<AddMediaCommand, Result>(addMediaCommand);

        var message = "Media successfully added.";
        if (addMediaResult.IsFailure)
        {
            _logger.LogError(addMediaResult.Error.ToString());
            message = BuildErrorMessage(addMediaResult);
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }

    private static string BuildErrorMessage(Result result)
    {
        const string message = "Adding media failed: ";
        return message + result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Website was not found",
            Errors.Media.MediaWithScrapeTargetExistsErrorCode => "Media for this URL exists",
            Errors.Media.MediaWithNameExistsErrorCode => "Media with this name exists",
            Errors.Scraper.ScrapeFailedErrorCode => "Scraping for media failed",
            Errors.Scraper.ScrapeMediaNameFailedErrorCode => "Scraping for media name failed",
            Errors.Validation.InvariantViolationErrorCode => "Creating entity failed",
            _ => "Something went wrong"
        };
    }
}