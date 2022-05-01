using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper;

public class PlaywrightScraper : IScraper
{
    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        Microsoft.Playwright.Program.Main(new[] {"install"});
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(scrapeInstruction.Url + scrapeInstruction.RelativeUrl);

        var container = await page.WaitForSelectorAsync("div.chapter-container", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");

        var newestLink = await container.QuerySelectorAsync("div.chapter-row >> div.col >> a");
        if (newestLink == null)
            return Errors.Scraper.ScrapeFailedError("Newest chapter link element could not be selected.");

        var relativeChapterUrl = await newestLink.GetAttributeAsync("href");
        if (relativeChapterUrl == null)
            return Errors.Scraper.ScrapeFailedError("Newest link element does not contain a chapter url");

        var releaseNumbersResult = ReleaseNumberExtractor.ExtractReleaseNumbers(relativeChapterUrl);
        if (releaseNumbersResult.IsFailure)
            return releaseNumbersResult.Error;

        return new ScrapedMediaRelease(
            scrapeInstruction.MediaName,
            scrapeInstruction.Url + relativeChapterUrl,
            releaseNumbersResult.Value.Major,
            releaseNumbersResult.Value.Minor
        );
    }
}