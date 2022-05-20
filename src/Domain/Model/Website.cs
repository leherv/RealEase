using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class Website : AggregateRoot
{
    public string Name { get; }
    public WebsiteUrl Url { get; }
    
    // for ef core only
    private Website(Guid id) : base(id)
    {
    }
    
    private Website(Guid id, WebsiteUrl url, string name) : base(id)
    {
        Url = url;
        Name = name;
    }
    
    public static Result<Website> Create(Guid id, WebsiteUrl url, string name)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(name, nameof(name))
            .ValidateAndCreate(() => new Website(id, url, name));
    }
}