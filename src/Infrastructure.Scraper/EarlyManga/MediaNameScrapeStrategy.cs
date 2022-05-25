using System.Threading.Tasks;
using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.EarlyManga;

internal class MediaNameScrapeStrategy : IMediaNameScrapeStrategy
{
    public async Task<Result<ScrapedMediaName>> Execute(IPage page, ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.manga-container", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing name information did not appear.");
        
        var nameSpan = await container.QuerySelectorAsync("h6.card-header > span:nth-child(2)");
        if (nameSpan == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing media name could not be selected.");

        var mangaName = await nameSpan.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}