using Application.Ports.Scraper;
using FakeItEasy;

namespace Application.Test.Fixture.Thens;

public class ThenTheScraper
{
    public IScraper Scraper;

    public ThenTheScraper(IScraper scraper)
    {
        Scraper = scraper;
    }

    public void HasBeenCalledWithScrapeInstructionOnce(ScrapeInstruction scrapeInstruction)
    {
        A.CallTo(() => Scraper.Scrape(A<ScrapeInstruction>.That.Matches(instruction => instruction.Equals(scrapeInstruction))))
            .MustHaveHappenedOnceExactly();
    }

    public void HasBeenCalledXTimes(int x)
    {
        A.CallTo(() => Scraper.Scrape(A<ScrapeInstruction>._)).MustHaveHappened(x, Times.Exactly);
    }
}