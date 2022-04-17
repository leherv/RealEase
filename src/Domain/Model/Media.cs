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
    
    private Media(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Result<Media> Create(Guid id, string name)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(name, nameof(name))
            .ValidateAndCreate(() => new Media(id, name));
    }

    public Result PublishNewRelease(Release release)
    {
        if (NewestRelease == null || release.IsNewerThan(NewestRelease))
        {
            NewestRelease = release;
            AddDomainEvent(new NewReleasePublished(release.Link, Name));
            return Result.Success();
        }

        return Errors.Media.PublishNewReleaseFailedError(release);
    }
}