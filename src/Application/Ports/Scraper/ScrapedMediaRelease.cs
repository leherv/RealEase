using Domain.Model;
using Domain.Results;

namespace Application.Ports.Scraper;

public record ScrapedMediaRelease(
    string UrlToResource,
    int MajorReleaseNumber,
    int? MinorReleaseNumber = 0)
{
    public Result<Release> ToDomain(DateTime utcNow)
    {
        var releaseNumberResult = ReleaseNumber.Create(MajorReleaseNumber, MinorReleaseNumber ?? 0);
        if (releaseNumberResult.IsFailure)
            return releaseNumberResult.Error;

        var resourceUrlResult = ResourceUrl.Create(UrlToResource);
        if (resourceUrlResult.IsFailure)
            return resourceUrlResult.Error;
        
        return Release.Create(releaseNumberResult.Value, resourceUrlResult.Value, utcNow);
    }
}