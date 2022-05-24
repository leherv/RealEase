namespace Domain.Model;

public record Release
{
    public ReleaseNumber ReleaseNumber { get; }
    public ResourceUrl ResourceUrl { get; }
    public DateTime Created { get; }

    private Release(ReleaseNumber releaseNumber, ResourceUrl resourceUrl, DateTime created)
    {
        ReleaseNumber = releaseNumber;
        ResourceUrl = resourceUrl;
        Created = created;
    }
    
    // only for ef core
    private Release(DateTime created)
    {
        Created = created;
    }

    public static Release Create(ReleaseNumber releaseNumber, ResourceUrl link, DateTime created)
    {
        return new Release(releaseNumber, link, created);
    }

    public bool IsNewerThan(Release release)
    {
        return ReleaseNumber.IsNewerThan(release.ReleaseNumber);
    }
}