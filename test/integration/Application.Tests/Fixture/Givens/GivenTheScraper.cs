using Application.Ports.Scraper;
using Domain.Results;
using FakeItEasy;

namespace Application.Test.Fixture.Givens;

public class GivenTheScraper
{
    public IScraper Scraper;

    public GivenTheScraper(IScraper scraper)
    {
        Scraper = scraper;
    }

    public void ScrapeForMediaWithNameReturns(string mediaName, Result<ScrapedMediaRelease> scrapeResult)
    {
        A.CallTo(() => Scraper.Scrape(A<ScrapeInstruction>.That.Matches(scrapeInstruction => scrapeInstruction.MediaName.ToLower().Equals(mediaName.ToLower()))))
            .Returns(scrapeResult);
    }
}