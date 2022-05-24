using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Adapters;

public class PlaywrightMediaNameScraper : IMediaNameScraper
{
    public async Task<Result<ScrapedMediaName>> ScrapeMediaName(ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(scrapeMediaNameInstruction.ResourceUrl);
        
        var container = await page.WaitForSelectorAsync("div.manga-container", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing name information did not appear.");
        
        var nameSpan = await container.QuerySelectorAsync("h6.card-header > span:nth-child(2)");
        if (nameSpan == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Newest chapter link element could not be selected.");

        var mangaName = await nameSpan.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}