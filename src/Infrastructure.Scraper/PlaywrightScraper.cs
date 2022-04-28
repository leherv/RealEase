using Application.Ports.Scraper;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper;

public class PlaywrightScraper : IScraper
{
    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
        var page = await browser.NewPageAsync();
        await page.GotoAsync("https://playwright.dev/dotnet");
        await page.ScreenshotAsync(new() { Path = "screenshot.png" });
        throw new Exception();
    }
}