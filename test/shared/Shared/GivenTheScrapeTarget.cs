using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheScrapeTarget
{
    public List<ScrapeTarget> ScrapeTargets;
    public ScrapeTarget MartialPeakEarlyManga { get; }
    public ScrapeTarget MartialPeakManganato { get; }
    public ScrapeTarget BorutoManganato { get; }
    public ScrapeTarget BorutoEarlyManga { get; }
    public ScrapeTarget HunterXHunterEarlyManga { get; }
    public ScrapeTarget SoloLevelingEarlyManga { get; }

    public GivenTheScrapeTarget(GivenTheWebsite givenTheWebsite)
    {
        HunterXHunterEarlyManga = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("manga/hunter-x-hunter").Value
        ).Value;
        
        SoloLevelingEarlyManga = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("manga/solo-leveling").Value
        ).Value;
        
        MartialPeakEarlyManga = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("manga/martial-peak/").Value
        ).Value;
        
        MartialPeakEarlyManga = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("manga/martial-peak/").Value
        ).Value;
        MartialPeakManganato = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("manga/manga-bn978870").Value
        ).Value;
        
        BorutoManganato = Create(
            Guid.NewGuid(),
            givenTheWebsite.Manganato,
            RelativeUrl.Create("/manga-tl971246").Value
        ).Value;
        
        BorutoEarlyManga = Create(
            Guid.NewGuid(),
            givenTheWebsite.EarlyManga,
            RelativeUrl.Create("/manga/boruto-naruto-next-generations").Value
        ).Value;

        ScrapeTargets = new List<ScrapeTarget>
        {
            HunterXHunterEarlyManga,
            SoloLevelingEarlyManga,
            MartialPeakEarlyManga,
            BorutoManganato
        };
    }

    public static Result<ScrapeTarget> Create(
        Guid? id = null,
        Website? website = null,
        RelativeUrl? relativeUrl = null
    )
    {
        return ScrapeTarget.Create(
            id ?? Guid.NewGuid(),
            website ?? Website.Create(
                Guid.NewGuid(),
                WebsiteUrl.Create("https://earlymng.org/").Value,
                "earlymanga").Value,
            relativeUrl ?? RelativeUrl.Create("manga/martial-peak/").Value
        );
    }
}