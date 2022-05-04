using Application.Ports.Scraper;
using FakeItEasy;

namespace Application.Test.Fixture.Thens;

public class ThenTheScraper
{
    private readonly IScraper _scraper;

    public ThenTheScraper(IScraper scraper)
    {
        _scraper = scraper;
    }

    public void HasBeenCalledWithScrapeInstructionOnce(ScrapeInstruction scrapeInstruction)
    {
        A.CallTo(() => _scraper.Scrape(A<ScrapeInstruction>.That.Matches(instruction => instruction.Equals(scrapeInstruction))))
            .MustHaveHappenedOnceExactly();
    }

    public void HasBeenCalledXTimes(int x)
    {
        A.CallTo(() => _scraper.Scrape(A<ScrapeInstruction>._)).MustHaveHappened(x, Times.Exactly);
    }
}