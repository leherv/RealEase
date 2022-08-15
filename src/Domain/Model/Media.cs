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
        if (ReleaseCanBePublished(release))
        {
            NewestRelease = release;
            AddDomainEvent(new NewReleasePublished(Id, Name, release.ResourceUrl.Value));
            return Result.Success();
        }

        return Errors.Media.PublishNewReleaseFailedError(release);
    }

    public bool ReleaseCanBePublished(Release release)
    {
        return NewestRelease == null || release.IsNewerThan(NewestRelease);
    }

    public Result AddScrapeTarget(ScrapeTarget scrapeTarget, string scrapedMediaName)
    {
        if (ScrapeTargetAlreadyConfigured(scrapeTarget))
            return Errors.Media.ScrapeTargetExistsError(Name);

        if (!ReferencesSameMedia(scrapedMediaName))
            return Errors.Media.ScrapeTargetReferencesOtherMediaError(Name, scrapedMediaName);

        _scrapeTargets.Add(scrapeTarget);

        return Result.Success();
    }
    
    public bool ScrapeTargetAlreadyConfigured(ScrapeTarget scrapeTarget)
    {
        return _scrapeTargets.Any(existingScrapeTarget =>
            existingScrapeTarget.RelativeUrl == scrapeTarget.RelativeUrl &&
            existingScrapeTarget.WebsiteId == scrapeTarget.WebsiteId);
    }
    
    private bool ReferencesSameMedia(string scrapedMediaName)
    {
        if (string.IsNullOrEmpty(scrapedMediaName))
            return false;
        
        return Name.Equals(scrapedMediaName, StringComparison.InvariantCultureIgnoreCase) ||
               Name.Contains(scrapedMediaName, StringComparison.InvariantCultureIgnoreCase) ||
               scrapedMediaName.Contains(Name, StringComparison.InvariantCultureIgnoreCase);
    }

    public bool RemoveScrapeTarget(Guid scrapeTargetId)
    {
        var scrapeTarget = ScrapeTargets
            .SingleOrDefault(scrapeTarget => scrapeTarget.Id.Equals(scrapeTargetId));

        return scrapeTarget != null && _scrapeTargets.Remove(scrapeTarget);
    }

    public bool HasScrapeTargets => ScrapeTargets.Any();
}