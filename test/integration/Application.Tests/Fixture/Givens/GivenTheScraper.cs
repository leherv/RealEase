using Application.Ports.Scraper;
using Domain.Results;
using FakeItEasy;

namespace Application.Test.Fixture.Givens;

public class GivenTheScraper
{
    private readonly IScraper _scraper;

    public GivenTheScraper(IScraper scraper)
    {
        _scraper = scraper;
    }

    public void ScrapeForMediaWithNameReturns(string mediaName, Result<ScrapedMediaRelease> scrapeResult)
    {
        A.CallTo(() => _scraper.Scrape(A<ScrapeInstruction>.That.Matches(scrapeInstruction =>
                scrapeInstruction.MediaName.ToLower().Equals(mediaName.ToLower()))))
            .Returns(scrapeResult);
    }

    public void ScrapeForWebsiteReturns(string websiteName, Result<ScrapedMediaRelease> scrapeResult)
    {
        A.CallTo(() => _scraper.Scrape(A<ScrapeInstruction>.That.Matches(scrapeInstruction =>
                scrapeInstruction.WebsiteName.ToLower().Equals(websiteName.ToLower()))))
            .Returns(scrapeResult);
    }

    public void ScrapeForAnyMediaReturns(Result<ScrapedMediaRelease> scrapeResult)
    {
        A.CallTo(() => _scraper.Scrape(A<ScrapeInstruction>._)).Returns(scrapeResult);
    }
}