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
        await WaitForChapterListToLoad(page);
        await MoveNewestChapterToTheTop(page);
        await WaitForChapterListToLoad(page);
        
        var newestListEntry = await page.QuerySelectorAsync("ul.list-body >> li");
        if (newestListEntry == null)
            return Errors.Scraper.ScrapeFailedError("Newest list element could not be selected.");
        
        var relativeUrl = await newestListEntry.GetAttributeAsync("data-href");
        if (relativeUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest list element does not contain a chapter url");

        var episodeInfo = await newestListEntry.QuerySelectorAsync("a.info__label");
        if (episodeInfo == null)
            return Errors.Scraper.ScrapeFailedError("Newest list element does not contain a episode number");
        var episodeInfoText = await episodeInfo.InnerTextAsync();
        
        var releaseNumbersResult = ReleaseNumberExtractor.ExtractReleaseNumbers(episodeInfoText);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;
        
        return new ScrapedMediaRelease(
            UriCombinator.Combine(scrapeInstruction.WebsiteUrl, relativeUrl),
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }

    private static async Task WaitForChapterListToLoad(IPage page)
    {
        await page.Locator("ul.list-body >> li >> nth=0").WaitForAsync();
    }

    private static async Task MoveNewestChapterToTheTop(IPage page)
    {
        await page.Locator("a.sort-btn").ClickAsync();
    }
    
}