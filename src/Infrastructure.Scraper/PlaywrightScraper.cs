using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper;

public class PlaywrightScraper : IScraper
{
    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(scrapeInstruction.Url);

        var container = await page.WaitForSelectorAsync("div.panel-story-chapter-list", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");

        var newestLink = await container.QuerySelectorAsync("ul.row-content-chapter >> li.a-h >> a");
        if (newestLink == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");

        var chapterUrl = await newestLink.GetAttributeAsync("href");
        if (chapterUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest link element does not contain a chapter url");

        var releaseNumbersResult = ReleaseNumberExtractor.ExtractReleaseNumbers(chapterUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;

        return new ScrapedMediaRelease(
            scrapeInstruction.MediaName,
            chapterUrl,
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}