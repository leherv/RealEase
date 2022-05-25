using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record WebsiteUrl(string Value)
{
    public static Result<WebsiteUrl> Create(string url)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(url, nameof(url))
            .ValidateAndCreate(() => new WebsiteUrl(url));
    }
}