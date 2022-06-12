using Application.Ports.General;
using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Adapters;

public class PlaywrightScraper : IScraper
{
    private readonly IApplicationLogger _applicationLogger;
    
    public PlaywrightScraper(IApplicationLogger applicationLogger)
    {
        _applicationLogger = applicationLogger;
    }

    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        var scrapeStrategy = ReleaseScrapeStrategyFactory.Create(scrapeInstruction.WebsiteName);
        try
        {
            // Program.Main(new[] {"install"});
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Firefox.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(scrapeInstruction.ResourceUrl);

            return await scrapeStrategy.Execute(page, scrapeInstruction);
        }
        catch (TimeoutException timeoutException)
        {
            _applicationLogger.LogWarning(timeoutException,
                $"Scraping for Media with name {scrapeInstruction.MediaName} and ScrapeTarget with websiteName {scrapeInstruction.WebsiteName} and ResourceUrl {scrapeInstruction.ResourceUrl} failed.");
            return Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError(timeoutException.Message));
        }
    }
}