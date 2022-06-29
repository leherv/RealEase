using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheMedia
{
    public List<Media> MediaList { get; }
    public List<Media> MediaWithScrapeTargets { get; }
    public List<Media> MediaWithScrapeTargetsPointingToInActiveWebsites { get; }
    public Media WithoutSubscribersWithoutReleasesWithoutScrapeTarget { get; }
    public Media WithoutSubscriberWithoutReleases { get; }
    public Media WithSubscriberWithReleases { get; }
    public Release CurrentRelease { get; }
    public Media WithSubscriberWithoutRelease { get; }
    public Media NotPersistedMedia { get; }
    public Media WithSubscriberWithTwoScrapeTargets { get; }
    public Media WithInActiveWebsite { get; }

    public GivenTheMedia(GivenTheScrapeTarget givenTheScrapeTarget, GivenTheWebsite givenTheWebsite)
    {
        WithSubscriberWithoutRelease = Create(
            Guid.NewGuid(),
            "Hunter x Hunter"
        ).Value;
        WithSubscriberWithoutRelease.AddScrapeTarget(givenTheScrapeTarget.HunterXHunterEarlyManga);

        WithSubscriberWithReleases = Create(
            Guid.NewGuid(),
            "Martial Peak"
        ).Value;
        WithSubscriberWithReleases.AddScrapeTarget(givenTheScrapeTarget.MartialPeakEarlyManga);
        CurrentRelease = GivenTheRelease.Create(
            ReleaseNumber.Create(3, 0).Value,
            ResourceUrl.Create("https://www.thisIsATest.com/chapter/3").Value
        ).Value;
        WithSubscriberWithReleases.PublishNewRelease(CurrentRelease);

        WithoutSubscriberWithoutReleases = Create(
            Guid.NewGuid(),
            "Solo Leveling"
        ).Value;
        WithoutSubscriberWithoutReleases.AddScrapeTarget(givenTheScrapeTarget.SoloLevelingEarlyManga);

        WithoutSubscribersWithoutReleasesWithoutScrapeTarget = Create(
            Guid.NewGuid(),
            "Boruto"
        ).Value;

        WithSubscriberWithTwoScrapeTargets = Create(
            Guid.NewGuid(),
            "Magic Emperor"
        ).Value;
        WithSubscriberWithTwoScrapeTargets.AddScrapeTarget(givenTheScrapeTarget.MagicEmperorEarlyManga);
        WithSubscriberWithTwoScrapeTargets.AddScrapeTarget(givenTheScrapeTarget.MagicEmperorManganato);

        WithInActiveWebsite = Create(
            Guid.NewGuid(),
            "Faceless"
        ).Value;
        WithInActiveWebsite.AddScrapeTarget(givenTheScrapeTarget.FacelessMangaWalker);
        
        MediaList = new List<Media>
        {
            WithSubscriberWithoutRelease,
            WithSubscriberWithReleases,
            WithSubscriberWithTwoScrapeTargets,
            WithoutSubscriberWithoutReleases,
            WithoutSubscribersWithoutReleasesWithoutScrapeTarget,
            WithInActiveWebsite
        };

        MediaWithScrapeTargets = new List<Media>
        {
            WithSubscriberWithTwoScrapeTargets,
            WithSubscriberWithoutRelease,
            WithSubscriberWithReleases,
            WithoutSubscriberWithoutReleases,
        };

        MediaWithScrapeTargetsPointingToInActiveWebsites = new List<Media>
        {
            WithInActiveWebsite
        };

        var notPersistedScrapeTarget = GivenTheScrapeTarget.Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("/manga/tower-of-god").Value
        ).Value;
        NotPersistedMedia = Create(Guid.NewGuid(), "Tower of God").Value;
        NotPersistedMedia.AddScrapeTarget(notPersistedScrapeTarget);
    }

    public static Result<Media> Create(
        Guid? id = null,
        string? mediaName = null
    )
    {
        return Media.Create(
            id ?? Guid.NewGuid(),
            mediaName ?? "Hunter x Hunter"
        );
    }
}