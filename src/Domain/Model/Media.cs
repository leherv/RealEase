using Domain.ApplicationErrors;
using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Model.Events;
using Domain.Results;

namespace Domain.Model;

public class Media : AggregateRoot
{
    public string Name { get; }
    public Release? NewestRelease { get; private set; }
    public ScrapeTarget? ScrapeTarget { get; }

    // only for ef core
    private Media(Guid id) : base(id)
    {
    }

    private Media(Guid id, string name, ScrapeTarget? scrapeTarget) : base(id)
    {
        Name = name;
        ScrapeTarget = scrapeTarget;
    }

    public static Result<Media> Create(Guid id, string name, ScrapeTarget? scrapeTarget)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(name, nameof(name))
            .ValidateAndCreate(() => new Media(id, name, scrapeTarget));
    }

    public Result PublishNewRelease(Release release)
    {
        if (NewestRelease == null || release.IsNewerThan(NewestRelease))
        {
            NewestRelease = release;
            AddDomainEvent(new NewReleasePublished(Id, Name, release.Link));
            return Result.Success();
        }

        return Errors.Media.PublishNewReleaseFailedError(release);
    }
}