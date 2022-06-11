using Application.Ports.Scraper;
using Domain.ApplicationErrors;
using Domain.Results;
using Infrastructure.Scraper.Base;
using Microsoft.Playwright;

namespace Infrastructure.Scraper.Manganato;

internal class MediaNameScrapeStrategy : IMediaNameScrapeStrategy
{
    public async Task<Result<ScrapedMediaName>> Execute(IPage page, ScrapeMediaNameInstruction scrapeMediaNameInstruction)
    {
        var container = await page.WaitForSelectorAsync("div.container-main", new PageWaitForSelectorOptions
        {
            State = WaitForSelectorState.Visible
        });
        if (container == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing name information did not appear.");
        
        var nameH1 = await container.QuerySelectorAsync("div.panel-story-info > div.story-info-right > h1");
        if (nameH1 == null)
            return Errors.Scraper.ScrapeMediaNameFailedError("Element containing media name could not be selected.");

        var mangaName = await nameH1.InnerTextAsync();
        
        return Result<ScrapedMediaName>.Success(new ScrapedMediaName(mangaName));
    }
}