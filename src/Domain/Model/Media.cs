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

    private List<ScrapeTarget> _scrapeTargets = new();
    public IReadOnlyCollection<ScrapeTarget> ScrapeTargets => _scrapeTargets;

    private Media(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Result<Media> Create(Guid id, string name)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(name, nameof(name))
            .ValidateAndCreate(() =>
                new Media(id, name)
            );
    }

    public Result PublishNewRelease(Release release)
    {
        if (NewestRelease == null || release.IsNewerThan(NewestRelease))
        {
            NewestRelease = release;
            AddDomainEvent(new NewReleasePublished(Id, Name, release.ResourceUrl.Value));
            return Result.Success();
        }

        return Errors.Media.PublishNewReleaseFailedError(release);
    }

    public Result AddScrapeTarget(ScrapeTarget scrapeTarget)
    {
        if (ScrapeTargetAlreadyConfigured(scrapeTarget))
            return Errors.Media.ScrapeTargetExistsError(Name);

        _scrapeTargets.Add(scrapeTarget);

        return Result.Success();
    }

    public bool ScrapeTargetAlreadyConfigured(ScrapeTarget scrapeTarget)
    {
        return _scrapeTargets.Any(existingScrapeTarget =>
            existingScrapeTarget.RelativeUrl == scrapeTarget.RelativeUrl &&
            existingScrapeTarget.WebsiteId == scrapeTarget.WebsiteId);
    }

    public bool HasScrapeTargets => ScrapeTargets.Any();
}