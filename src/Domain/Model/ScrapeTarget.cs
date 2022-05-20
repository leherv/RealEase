using Domain.Invariants;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class ScrapeTarget : Entity
{
    public Guid WebsiteId { get; }
    public RelativeUrl RelativeUrl { get; }

    // only for ef core
    private ScrapeTarget(Guid id) : base(id) {}
    
    private ScrapeTarget(Guid id, Website website, RelativeUrl relativeUrl) : base(id)
    {
        WebsiteId = website.Id;
        RelativeUrl = relativeUrl;
    }
    
    public static Result<ScrapeTarget> Create(Guid id, Website website, RelativeUrl relativeUrl)
    {
        return Invariant.Create
            .ValidateAndCreate(() => new ScrapeTarget(id, website, relativeUrl));
    }
}