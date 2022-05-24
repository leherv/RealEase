using Application.Ports.Scraper;
using Domain.Results;
using Infrastructure.Scraper.Base;
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

        var mediaNameScrapeStrategy = MediaNameScrapeStrategyFactory.Create(scrapeMediaNameInstruction.WebsiteName);

        return await mediaNameScrapeStrategy.Execute(page, scrapeMediaNameInstruction);
    }
}