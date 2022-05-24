using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record WebsiteUrl(string Value)
{
    public static Result<WebsiteUrl> Create(string url)
    {
        var preparedUrl = url.TrimEnd('/');
        return Invariant.Create
            .NotNullOrWhiteSpace(preparedUrl, nameof(preparedUrl))
            .ValidateAndCreate(() => new WebsiteUrl(preparedUrl));
    }
}