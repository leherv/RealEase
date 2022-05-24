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
            message = "Adding media failed";
            _logger.LogWarning(addMediaResult.Error.ToString());
            if (addMediaResult.Error.Code == Errors.Media.MediaWithNameExistsErrorCode)
                message += " as media with this name already exists";
            if (addMediaResult.Error.Code == Errors.Media.MediaWithScrapeTargetExistsErrorCode)
                message += " as another media already is configured for this URL";
            if (addMediaResult.Error.Code == Errors.General.NotFoundErrorCode)
                message += $" as website with {websiteName} could not be found";
        }
        
        await Context.Message.Channel.SendMessageAsync(message);
    }
}