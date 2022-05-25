using System.Threading.Tasks;
using Application.Ports.Scraper;
using Domain.Results;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Base;

internal interface IMediaNameScrapeStrategy
{
    Task<Result<ScrapedMediaName>> Execute(IPage page, ScrapeMediaNameInstruction scrapeMediaNameInstruction);
}