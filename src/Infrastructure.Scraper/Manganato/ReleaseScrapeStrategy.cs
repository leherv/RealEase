using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Infrastructure.Scraper.Shared;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Manganato;

internal class ReleaseScrapeStrategy : IReleaseScrapeStrategy
{
    public async Task<Result<ScrapedMediaRelease>> Execute(IPage page, ScrapeInstruction scrapeInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.panel-story-chapter-list", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");
        
        var newestLink = await container.QuerySelectorAsync("li > a");
        if (newestLink == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");
        
        var chapterUrl = await newestLink.GetAttributeAsync("href");
        if (chapterUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest link element does not contain a chapter url");
        
        var releaseNumbersResult = UriReleaseNumberExtractor.ExtractReleaseNumbers(chapterUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;
        
        return new ScrapedMediaRelease(
            chapterUrl,
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}