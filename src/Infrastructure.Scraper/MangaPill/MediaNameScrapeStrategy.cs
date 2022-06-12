using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.MangaPill;

internal class MediaNameScrapeStrategy : IMediaNameScrapeStrategy
{
    public async Task<Result<ScrapedMediaName>> Execute(IPage page, ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        var container = await page.WaitForSelectorAsync("div[data-filter-list] a", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeFailedError("Element containing chapters did not appear.");
        
        var nameH1 = await page.WaitForSelectorAsync("h1", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (nameH1 == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing media name could not be selected.");
        
        var mangaName = await nameH1.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}