using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheMedia
{
    public List<Media> MediaList { get; }
    public List<Media> MediaWithScrapeTargets { get; }
    public Media WithoutSubscribersWithoutReleasesWithoutScrapeTarget { get; }
    public Media WithoutSubscriberWithoutReleases { get; }
    public Media WithSubscriberWithReleases { get; }
    public Release CurrentRelease { get; }
    public Media WithSubscriberWithoutRelease { get; }

    public GivenTheMedia(GivenTheScrapeTarget givenTheScrapeTarget)
    {
        WithSubscriberWithoutRelease = Create(
            Guid.NewGuid(),
            "Hunter x Hunter",
            givenTheScrapeTarget.HunterXHunterEarlyManga
        ).Value;
        
        WithSubscriberWithReleases = Create(
            Guid.NewGuid(),
            "Martial Peak",
            givenTheScrapeTarget.MartialPeakEarlyManga
        ).Value;
        CurrentRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(3, 0).Value, "https://www.thisIsATest.com/chapter/3"
        ).Value;
        WithSubscriberWithReleases.PublishNewRelease(CurrentRelease);
        
        WithoutSubscriberWithoutReleases =  Create(
            Guid.NewGuid(),
            "Solo Leveling",
            givenTheScrapeTarget.SoloLevelingEarlyManga
        ).Value;
        
        WithoutSubscribersWithoutReleasesWithoutScrapeTarget = Create(
            Guid.NewGuid(),
            "Naruto"
        ).Value;

        MediaList = new List<Media>
        {
            WithSubscriberWithoutRelease,
            WithSubscriberWithReleases,
            WithoutSubscriberWithoutReleases,
            WithoutSubscribersWithoutReleasesWithoutScrapeTarget
        };

        MediaWithScrapeTargets = new List<Media>
        {
            WithSubscriberWithoutRelease,
            WithSubscriberWithReleases,
            WithoutSubscriberWithoutReleases
        };
    }

    public static Result<Media> Create(
        Guid? id = null,
        string? mediaName = null,
        ScrapeTarget? scrapeTarget = null
    )
    {
        return Media.Create(
            id ?? Guid.NewGuid(),
            mediaName ?? "Hunter x Hunter",
            scrapeTarget
        );
    }
}