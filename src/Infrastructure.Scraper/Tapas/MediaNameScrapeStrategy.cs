using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Tapas;

internal class MediaNameScrapeStrategy : IMediaNameScrapeStrategy
{
    public async Task<Result<ScrapedMediaName>> Execute(IPage page, ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.title-wrapper", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing name information did not appear.");
        
        var nameLink = await container.QuerySelectorAsync("a");
        if (nameLink == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing media name could not be selected.");

        var mangaName = await nameLink.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}