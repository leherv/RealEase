using Application.Ports.Scraper;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Adapters;

public class PlaywrightScraper : IScraper
{
    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        // Program.Main(new[] {"install"});
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        await page.GotoAsync(scrapeInstruction.ResourceUrl);

        var scrapeStrategy = ReleaseScrapeStrategyFactory.Create(scrapeInstruction.WebsiteName);
        
        return await scrapeStrategy.Execute(page, scrapeInstruction);
    }
}