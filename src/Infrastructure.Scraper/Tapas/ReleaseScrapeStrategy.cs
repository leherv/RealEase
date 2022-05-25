using System.Threading.Tasks;
using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Infrastructure.Scraper.Shared;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Tapas;

internal class ReleaseScrapeStrategy : IReleaseScrapeStrategy
{
    public async Task<Result<ScrapedMediaRelease>> Execute(IPage page, ScrapeInstruction scrapeInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.episode-list", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");
        
        var newestListEntry = await container.QuerySelectorAsync("ul li:last-of-type");
        if (newestListEntry == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");
        
        var relativeUrl = await newestListEntry.GetAttributeAsync("data-href");
        if (relativeUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest list element does not contain a chapter url");

        var episodeInfo = await newestListEntry.QuerySelectorAsync("a.info__title");
        if (episodeInfo == null)
            return Errors.Scraper.ScrapeFailedError("Newest list element does not contain a episode number");
        
        var releaseNumbersResult = ReleaseNumberExtractor.ExtractReleaseNumbers(relativeUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;
        
        return new ScrapedMediaRelease(
            UriCombinator.Combine(scrapeInstruction.WebsiteUrl, relativeUrl),
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}