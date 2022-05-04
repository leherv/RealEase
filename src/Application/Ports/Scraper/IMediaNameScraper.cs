using Domain.Results;

namespace Application.Ports.Scraper;

public interface IMediaNameScraper
{
    Task<Result<ScrapedMediaName>> ScrapeMediaName(ScrapeMediaNameInstruction scrapeMediaNameInstruction);
}