using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryMedia;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class MediaReadRepository : IMediaReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public MediaReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<MediaDetails?> QueryById(Guid id)
    {
        var media = await FetchMediaForId(id);
        if (media == null)
            return null;

        var scrapeTargetDetails = await BuildScrapeTargetDetailsFor(media);
        var releaseDetails = BuildReleaseDetails(media.NewestRelease);

        return new MediaDetails(media.Id, media.Name, scrapeTargetDetails, releaseDetails);
    }

    private static ReleaseDetails? BuildReleaseDetails(Release? newestRelease)
    {
        return newestRelease == null
            ? null
            : new ReleaseDetails(
                newestRelease.ReleaseNumber.Major,
                newestRelease.ReleaseNumber.Minor,
                newestRelease.ResourceUrl.Value);
    }

    private async Task<List<ScrapeTargetDetails>> BuildScrapeTargetDetailsFor(Media media)
    {
        var scrapeTargetDetails = new List<ScrapeTargetDetails>();
        foreach (var scrapeTarget in media.ScrapeTargets)
        {
            var website = await FetchWebsiteForId(scrapeTarget.WebsiteId);

            scrapeTargetDetails.Add(new ScrapeTargetDetails(
                    scrapeTarget.Id,
                    website.Name,
                    website.Url.Value,
                    ResourceUrl.Create(website.Url, scrapeTarget.RelativeUrl).Value
                )
            );
        }

        return scrapeTargetDetails;
    }

    private async Task<Website?> FetchWebsiteForId(Guid websiteId)
    {
        return await _databaseContext.Websites.SingleOrDefaultAsync(website => website.Id == websiteId);
    }

    private async Task<Media?> FetchMediaForId(Guid id)
    {
        return await _databaseContext.Media
            .Include(media => media.ScrapeTargets)
            .SingleOrDefaultAsync(media => media.Id.Equals(id));
    }
}