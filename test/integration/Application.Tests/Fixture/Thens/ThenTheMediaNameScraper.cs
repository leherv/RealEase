using Application.Ports.Scraper;
using FakeItEasy;

namespace Application.Test.Fixture.Thens;

public class ThenTheMediaNameScraper
{
    private readonly IMediaNameScraper _mediaNameScraper;

    public ThenTheMediaNameScraper(IMediaNameScraper mediaNameScraper)
    {
        _mediaNameScraper = mediaNameScraper;
    }
    
    public void HasBeenCalledXTimes(int x)
    {
        A.CallTo(() => _mediaNameScraper.ScrapeMediaName(A<ScrapeMediaNameInstruction>._)).MustHaveHappened(x, Times.Exactly);
    }
}