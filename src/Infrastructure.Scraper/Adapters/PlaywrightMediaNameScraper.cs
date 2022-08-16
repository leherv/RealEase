using Application.Ports.General;
using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Adapters;

public class PlaywrightMediaNameScraper : IMediaNameScraper
{
    private readonly IApplicationLogger _applicationLogger;

    public PlaywrightMediaNameScraper(IApplicationLogger applicationLogger)
    {
        _applicationLogger = applicationLogger;
    }

    public async Task<Result<ScrapedMediaName>> ScrapeMediaName(ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            await page.GotoAsync(scrapeMediaNameInstruction.ResourceUrl);

            var mediaNameScrapeStrategy = MediaNameScrapeStrategyFactory.Create(scrapeMediaNameInstruction.WebsiteName);

            return await mediaNameScrapeStrategy.Execute(page, scrapeMediaNameInstruction);
        }
        catch (TimeoutException timeoutException)
        {
            _applicationLogger.LogWarning(timeoutException, $"Scraping for mediaName at url {scrapeMediaNameInstruction.ResourceUrl} and Website {scrapeMediaNameInstruction.WebsiteName} failed.");
            return Result<ScrapedMediaName>.Failure(Errors.Scraper.ScrapeFailedError(timeoutException.Message));
        }
    }
}