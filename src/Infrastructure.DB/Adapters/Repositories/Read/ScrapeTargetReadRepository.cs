using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryScrapeTargets;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class ScrapeTargetReadRepository : IScrapeTargetReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public ScrapeTargetReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Result<ScrapeTargets>> QueryScrapeTargetsFor(string mediaName)
    {
       
        var media = await _databaseContext.Media
            .Include(media => media.ScrapeTargets)
            .FirstOrDefaultAsync(media => media.Name == mediaName);
        
        if (media == null)
            return Errors.General.NotFound(nameof(Media));

        return await FetchScrapeTargets(media);
    }

    private async Task<Result<ScrapeTargets>> FetchScrapeTargets(Media media)
    {
        var scrapeTargetInformation = new List<ScrapeTargetInformation>();
        foreach (var scrapeTarget in media.ScrapeTargets)
        {
            var website =
                await _databaseContext.Websites.FirstOrDefaultAsync(website => scrapeTarget.WebsiteId == website.Id);
            if (website == null)
                return Errors.General.NotFound(nameof(Website));
            if(!website.Active)
                continue;

            scrapeTargetInformation.Add(new ScrapeTargetInformation(
                scrapeTarget.Id,
                website.Name,
                website.Url.Value,
                scrapeTarget.RelativeUrl.Value)
            );
        }

        return new ScrapeTargets(scrapeTargetInformation);
    }
}