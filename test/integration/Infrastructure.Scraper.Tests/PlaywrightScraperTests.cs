using Application.Ports.Scraper;
using FluentAssertions;
using Infrastructure.General.Adapters;
using Infrastructure.Scraper.Adapters;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Infrastructure.Scraper.Tests;

public class PlaywrightScraperTests
{
    private readonly PlaywrightScraper _sut;

    public PlaywrightScraperTests(ITestOutputHelper testOutputHelper)
    {
        using var loggerFactory = new LoggerFactory();
        loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
        var logger = loggerFactory.CreateLogger<ApplicationLogger>();
        _sut = new PlaywrightScraper(new ApplicationLogger(logger));
    }

    [Theory(Skip = "only manual run")]
    [InlineData("martial peak", "earlymanga", "https://earlym.org/", "https://earlym.org/manga/martial-peak")]
    [InlineData("naruto", "manganato", "https://readmanganato.com/", "https://readmanganato.com/manga-ng952689")] // Naruto
    [InlineData("one-piece", "mangapill", "https://mangapill.com/", "https://mangapill.com/manga/2/one-piece")]
    [InlineData("the beginning after the end", "tapas", "https://tapas.io/", "https://tapas.io/episode/1123711")]
    public async Task Scraping_must_return_success(
        string mediaName,
        string websiteName,
        string websiteUrl,
        string resourceUrl
    )
    {
        var scrapeInstruction = new ScrapeInstruction(
            mediaName,
            websiteName,
            websiteUrl,
            resourceUrl
        );

        var scrapeResult = await _sut.Scrape(scrapeInstruction);

        scrapeResult.IsSuccess.Should().BeTrue();
    }
}