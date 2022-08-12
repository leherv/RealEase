using Application.UseCases.Base;
using Application.UseCases.Media.QueryScrapeTargets;
using Discord.Commands;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Discord.Commands;

public class ListScrapeTargets : ModuleBase<SocketCommandContext>
{
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ILogger<ListScrapeTargets> _logger;

    internal ListScrapeTargets(ILogger<ListScrapeTargets> logger, IQueryDispatcher queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [Command("listScrapeTargets")]
    [Alias("lST")]
    internal async Task ListScrapeTargetsHandler(string mediaName)
    {
        var scrapeTargetsResult =
            await _queryDispatcher.Dispatch<ScrapeTargetsQuery, Result<ScrapeTargets>>(
                new ScrapeTargetsQuery(mediaName));

        string message;
        if (scrapeTargetsResult.IsFailure)
        {
            _logger.LogError(scrapeTargetsResult.Error.ToString());
            message = BuildErrorMessage(scrapeTargetsResult);
        }
        else
        {
            message = BuildSuccessMessage(scrapeTargetsResult.Value);
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }
    
    private static string BuildErrorMessage(Result result)
    {
        const string message = "Listing ScrapeTargets failed: ";
        return message + result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Entity was not found",
            _ => "Something went wrong"
        };
    }

    private static string BuildSuccessMessage(ScrapeTargets scrapeTargets)
    {
        var scrapeTargetInformation = scrapeTargets.ScrapeTargetInformation;
        var message = scrapeTargetInformation.Any()
            ? "ScrapeTargets:" +
              scrapeTargetInformation
                  .OrderBy(scrapeTargetInfo => scrapeTargetInfo.WebsiteName)
                  .Aggregate("", (current, scrapeTargetInfo) => current +
                                                                $"\n Website: {scrapeTargetInfo.WebsiteName}" +
                                                                $"\n Website URL: {scrapeTargetInfo.WebsiteUrl}" +
                                                                $"\n Relative URL: {scrapeTargetInfo.RelativeUrl}")
            : "\nNo ScrapeTargets yet.";

        return message;
    }
}