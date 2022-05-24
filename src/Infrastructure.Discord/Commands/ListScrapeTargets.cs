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

    public ListScrapeTargets(ILogger<ListScrapeTargets> logger, IQueryDispatcher queryDispatcher)
    {
        _logger = logger;
        _queryDispatcher = queryDispatcher;
    }

    [Command("listScrapeTargets")]
    [Alias("lST")]
    public async Task ListScrapeTargetsHandler(string mediaName)
    {
        var scrapeTargetsResult =
            await _queryDispatcher.Dispatch<ScrapeTargetsQuery, Result<ScrapeTargets>>(
                new ScrapeTargetsQuery(mediaName));

        string message;
        if (scrapeTargetsResult.IsFailure)
        {
            message = "Listing ScrapeTargets failed";
            _logger.LogWarning(scrapeTargetsResult.Error.ToString());
            if (scrapeTargetsResult.Error.Code == Errors.General.NotFoundErrorCode)
                message += " as media with this name could not be found";
            message += ".";
        }
        else
        {
            var scrapeTargetInformation = scrapeTargetsResult.Value.ScrapeTargetInformation;
            message = scrapeTargetInformation.Any()
                ? "ScrapeTargets:" +
                  scrapeTargetInformation
                      .OrderBy(scrapeTargetInfo => scrapeTargetInfo.WebsiteName)
                      .Aggregate("", (current, scrapeTargetInfo) => current +
                                                                    $"\n Website: {scrapeTargetInfo.WebsiteName}" +
                                                                    $"\n Website URL: {scrapeTargetInfo.WebsiteUrl}" +
                                                                    $"\n Relative URL: {scrapeTargetInfo.RelativeUrl}")
                : "\nNo websites available.";
        }

        await Context.Message.Channel.SendMessageAsync(message);
    }
}