using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record RelativeUrl(string Value)
{
    public static Result<RelativeUrl> Create(string url)
    {
        var preparedUrl = url.TrimStart('/');
        return Invariant.Create
            .NotNullOrWhiteSpace(preparedUrl, nameof(preparedUrl))
            .ValidateAndCreate(() => new RelativeUrl(preparedUrl));
    }
}