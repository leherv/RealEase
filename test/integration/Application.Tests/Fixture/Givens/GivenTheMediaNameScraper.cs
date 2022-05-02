using Application.Ports.Scraper;
using Domain.Results;
using FakeItEasy;

namespace Application.Test.Fixture.Givens;

public class GivenTheMediaNameScraper
{
    private readonly IMediaNameScraper _mediaNameScraper;

    public GivenTheMediaNameScraper(IMediaNameScraper mediaNameScraper)
    {
        _mediaNameScraper = mediaNameScraper;
    }
    
    public void ScrapeForAnyMediaReturns(Result<ScrapedMediaName> scrapeMediaNameResult)
    {
        A.CallTo(() => _mediaNameScraper.ScrapeMediaName(A<ScrapeMediaNameInstruction>._)).Returns(scrapeMediaNameResult);
    }
}