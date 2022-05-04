using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheScrapeTarget
{
    public List<ScrapeTarget> ScrapeTargets;
    public ScrapeTarget MartialPeakEarlyManga { get; }
    public ScrapeTarget NarutoManganato { get; }
    public ScrapeTarget HunterXHunterEarlyManga { get; }
    public ScrapeTarget SoloLevelingEarlyManga { get; }

    public GivenTheScrapeTarget(GivenTheWebsite givenTheWebsite)
    {
        HunterXHunterEarlyManga = Create(Guid.NewGuid(), givenTheWebsite.EarlyManga, "manga/hunter-x-hunter").Value;
        SoloLevelingEarlyManga = Create(Guid.NewGuid(), givenTheWebsite.EarlyManga, "manga/solo-leveling").Value;
        MartialPeakEarlyManga = Create(Guid.NewGuid(), givenTheWebsite.EarlyManga, "manga/martial-peak/").Value;
        NarutoManganato = Create(Guid.NewGuid(), givenTheWebsite.Manganato, "manga-ng952689").Value;
        
        ScrapeTargets = new List<ScrapeTarget>
        {
            HunterXHunterEarlyManga,
            SoloLevelingEarlyManga,
            MartialPeakEarlyManga,
            NarutoManganato
        };
    }

    public static Result<ScrapeTarget> Create(
        Guid? id = null,
        Website? website = null,
        string? relativeUrl = null
    )
    {
        return ScrapeTarget.Create(
            id ?? Guid.NewGuid(),
            website ?? Website.Create(Guid.NewGuid(), "https://earlymng.org/", "earlymanga").Value,
            relativeUrl ?? "manga/martial-peak/"
        );
    }
}