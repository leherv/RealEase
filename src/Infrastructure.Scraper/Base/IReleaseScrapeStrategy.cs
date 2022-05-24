using Application.Ports.Scraper;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Base;

internal interface IReleaseScrapeStrategy
{
    Task<Result<ScrapedMediaRelease>> Execute(IPage page, ScrapeInstruction scrapeInstruction);
}