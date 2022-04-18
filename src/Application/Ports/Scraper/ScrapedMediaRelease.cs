using Domain.Model;
using Domain.Results;

namespace Application.Ports.Scraper;

public record ScrapedMediaRelease(string MediaName, string UrlToResource, int MajorReleaseNumber,
    int? MinorReleaseNumber = 0)
{
    public Result<Release> ToDomain()
    {
        var releaseNumber = ReleaseNumber.Create(MajorReleaseNumber, MinorReleaseNumber ?? 0);

        return releaseNumber.IsFailure 
            ? releaseNumber.Error 
            : Release.Create(releaseNumber.Value, UrlToResource);
    }
}