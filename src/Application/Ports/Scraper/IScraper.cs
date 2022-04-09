using Domain.Results;

namespace Application.Ports.Scraper;

public interface IScraper
{
    Task<Result<ScrapeResult>> Scrape(ScrapeInstruction scrapeInstruction);
}