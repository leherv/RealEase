using Domain.Results;

namespace Application.Ports.Scraper;

public interface IScraper
{
    Task<Result<ScrapedMediaRelease>> Scrape(ScrapeInstruction scrapeInstruction);
}