using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class Website : AggregateRoot
{
    public string Name { get; }
    public string Url { get; }
    
    private Website(Guid id, string url, string name) : base(id)
    {
        Url = url;
        Name = name;
    }
    
    public static Result<Website> Create(Guid id, string url, string name)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(url, nameof(url))
            .ValidateAndCreate(() => new Website(id, url, name));
    }
}