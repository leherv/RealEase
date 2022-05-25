using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record RelativeUrl(string Value)
{
    public static Result<RelativeUrl> Create(string url)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(url, nameof(url))
            .ValidateAndCreate(() => new RelativeUrl(url));
    }
}