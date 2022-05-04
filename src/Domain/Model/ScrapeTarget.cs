using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class ScrapeTarget : Entity
{
    public Website Website { get; }
    public string RelativeUrl { get; }

    // only for ef core
    private ScrapeTarget(Guid id) : base(id) {}
    
    private ScrapeTarget(Guid id, Website website, string relativeUrl) : base(id)
    {
        Website = website;
        RelativeUrl = relativeUrl;
    }
    
    public static Result<ScrapeTarget> Create(Guid id, Website website, string relativeUrl)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl))
            .ValidateAndCreate(() => new ScrapeTarget(id, website, relativeUrl));
    }

    public string AbsoluteUrl => Path.Combine(Website.Url, RelativeUrl);
}