using Application.Ports.Scraper;
using FluentAssertions;
using Infrastructure.General.Adapters;
using Infrastructure.Scraper.Adapters;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Infrastructure.Scraper.Tests;

public class PlaywrightMediaNameScraperTests
{
    private readonly PlaywrightMediaNameScraper _sut;
    public PlaywrightMediaNameScraperTests(ITestOutputHelper testOutputHelper)
    {
        using var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
        var logger = loggerFactory.CreateLogger<ApplicationLogger>();
        _sut = new PlaywrightMediaNameScraper(new ApplicationLogger(logger));
    }
    
    [Theory(Skip = "only manual run")]
    [InlineData("earlymanga", "https://earlym.org/manga/martial-peak")]
    [InlineData("manganato", "https://readmanganato.com/manga-ng952689")] // Naruto
    [InlineData("mangapill", "https://mangapill.com/manga/2/one-piece")]
    [InlineData("tapas", "https://tapas.io/episode/1123711")]
    public async Task Scraping_must_return_success(string websiteName, string resourceUrl)
    {
        var scrapeMediaNameInstruction = new ScrapeMediaNameInstruction(websiteName, resourceUrl);
            
        var scrapeResult = await _sut.ScrapeMediaName(scrapeMediaNameInstruction);

        scrapeResult.IsSuccess.Should().BeTrue();
    }
}