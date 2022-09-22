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
        var container = await page.WaitForSelectorAsync("div.banner-info", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing name information did not appear.");
        
        var nameDiv = await container.QuerySelectorAsync("div.title");
        if (nameDiv == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing media name could not be selected.");

        var mangaName = await nameDiv.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}