using Application.Ports.Scraper;
using Domain.Results;

namespace Infrastructure.Scraper;

public class PlaywrightScraper : IScraper
{
    public Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction)
    {
        throw new NotImplementedException();
    }
}