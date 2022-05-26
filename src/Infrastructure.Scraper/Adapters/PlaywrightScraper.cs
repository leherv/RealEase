using System.Threading.Tasks;
using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Adapters;

public class PlaywrightScraper : IScraper
{
    private readonly ILogger<PlaywrightScraper> _logger;

    public PlaywrightScraper(ILogger<PlaywrightScraper> logger)
    {
        _logger = logger;
    }

    public async Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        try
        {
            // Program.Main(new[] {"install"});
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(scrapeInstruction.ResourceUrl);

            var scrapeStrategy = ReleaseScrapeStrategyFactory.Create(scrapeInstruction.WebsiteName);

            return await scrapeStrategy.Execute(page, scrapeInstruction);
        }
        catch (PlaywrightException playwrightException)
        {
            _logger.LogWarning(playwrightException,
                $"Scraping for Media with name {scrapeInstruction.MediaName} and ScrapeTarget with websiteName {scrapeInstruction.WebsiteName} and ResourceUrl {scrapeInstruction.ResourceUrl} failed.");
            return Result<ScrapedMediaRelease>.Failure(Errors.Scraper.ScrapeFailedError(playwrightException.Message));
        }
    }
}