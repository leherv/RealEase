using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class ScrapeTarget : AggregateRoot
{
    public string Url { get; }
    
    private ScrapeTarget(Guid id, string url) : base(id)
    {
        Url = url;
    }
    
    public static Result<ScrapeTarget> Create(Guid id, string url)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(url, nameof(url))
            .ValidateAndCreate(() => new ScrapeTarget(id, url));
    }
}