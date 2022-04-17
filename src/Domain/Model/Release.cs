using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record Release
{
    public ReleaseNumber ReleaseNumber { get; }
    public string Link { get; }

    private Release(ReleaseNumber releaseNumber, string link)
    {
        ReleaseNumber = releaseNumber;
        Link = link;
    }
    
    // only for ef core
    private Release() {}

    public static Result<Release> Create(ReleaseNumber releaseNumber, string link)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(link, nameof(link))
            .ValidateAndCreate(() => new Release(releaseNumber, link));
    }

    public bool IsNewerThan(Release release)
    {
        return ReleaseNumber.IsNewerThan(release.ReleaseNumber);
    }
}