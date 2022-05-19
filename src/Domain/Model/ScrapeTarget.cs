using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

// then make this a record
public class ScrapeTarget : Entity
{
    public Guid WebsiteId { get; }
    public string RelativeUrl { get; }

    // only for ef core
    private ScrapeTarget(Guid id) : base(id) {}
    
    private ScrapeTarget(Guid id, Website website, string relativeUrl) : base(id)
    {
        WebsiteId = website.Id;
        RelativeUrl = relativeUrl;
    }
    
    public static Result<ScrapeTarget> Create(Guid id, Website website, string relativeUrl)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(relativeUrl, nameof(relativeUrl))
            .ValidateAndCreate(() => new ScrapeTarget(id, website, relativeUrl));
    }
}