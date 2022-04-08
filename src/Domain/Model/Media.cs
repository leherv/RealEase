using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Model.Base;
using Domain.Results;

namespace Domain.Model;

public class Media : Entity
{
    public string Name { get; }
    
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
}