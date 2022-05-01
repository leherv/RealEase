using Shared;

namespace Application.Test.Fixture.Givens;

public class GivenTheData
{
    public GivenTheMedia Media { get; }
    public GivenTheSubscriber Subscriber { get; }
    public GivenTheRelease Release { get; }
    public GivenTheScrapeTarget ScrapeTarget { get; }
    public GivenTheWebsite Website { get; }

    public GivenTheData()
    {
        Website = new GivenTheWebsite();
        ScrapeTarget = new GivenTheScrapeTarget(Website);
        Media = new GivenTheMedia(ScrapeTarget);
        Subscriber = new GivenTheSubscriber(Media);
        Release = new GivenTheRelease();
    }
}