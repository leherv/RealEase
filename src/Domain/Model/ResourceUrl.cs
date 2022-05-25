using Domain.Invariants;
using Domain.Invariants.Extensions;
using Domain.Results;

namespace Domain.Model;

public record ResourceUrl(string Value)
{
    public static ResourceUrl Create(WebsiteUrl websiteUrl, RelativeUrl relativeUrl)
    {
        var preparedWebsiteUrl = websiteUrl.Value.TrimEnd('/');
        var preparedRelativeUrl = relativeUrl.Value.TrimStart('/');
        return new ResourceUrl($"{preparedWebsiteUrl}/{preparedRelativeUrl}");
    }

    public static Result<ResourceUrl> Create(string urlToResource)
    {
        return Invariant.Create
            .NotNullOrWhiteSpace(urlToResource, nameof(urlToResource))
            .ValidateAndCreate(() => new ResourceUrl(urlToResource));
    }
    
    public static Result<ResourceUrl> Create(WebsiteUrl websiteUrl, string relativeUrl)
    {
        var relativeUrlResult = RelativeUrl.Create(relativeUrl);
        if (relativeUrlResult.IsFailure)
            return relativeUrlResult.Error;
        return Create(websiteUrl, relativeUrlResult.Value);
    }
}