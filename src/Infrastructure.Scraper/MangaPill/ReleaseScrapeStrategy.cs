using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Infrastructure.Scraper.Shared;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.MangaPill;

internal class ReleaseScrapeStrategy : IReleaseScrapeStrategy
{
    public async Task<Result<ScrapedMediaRelease>> Execute(IPage page, ScrapeInstruction scrapeInstruction)
    {
        var newestLink = await page.WaitForSelectorAsync("div#chapters a", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (newestLink == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");
        
        var relativeChapterUrl = await newestLink.GetAttributeAsync("href");
        if (relativeChapterUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest link element does not contain a chapter url");

        var releaseNumbersResult = UriReleaseNumberExtractor.ExtractReleaseNumbers(relativeChapterUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;

        return new ScrapedMediaRelease(
            UriCombinator.Combine(scrapeInstruction.WebsiteUrl, relativeChapterUrl),
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}