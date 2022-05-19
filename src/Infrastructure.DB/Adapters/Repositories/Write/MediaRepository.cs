using Application.Ports.Persistence.Write;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Write;

public class MediaRepository : IMediaRepository
{
    private readonly DatabaseContext _databaseContext;

    public MediaRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Media?> GetByName(string mediaName)
    {
        return await _databaseContext.Media
            .Include(media => media.ScrapeTarget!)
            .SingleOrDefaultAsync(media => media!.Name.ToLower() == mediaName.ToLower());
    }

    public async Task<Media?> GetByUri(Website website, string relativeUrl)
    {
        return await _databaseContext.Media
            .Include(media => media.ScrapeTarget!)
            .SingleOrDefaultAsync(media => media.ScrapeTarget.WebsiteId == website.Id &&
                                           media.ScrapeTarget.RelativeUrl == relativeUrl);
    }

    public async Task<IReadOnlyCollection<Media>> GetAll()
    {
        return await _databaseContext.Media.ToListAsync();
    }

    public async Task AddMedia(Media media)
    {
        await _databaseContext.MediaDbSet.AddAsync(media);
    }
}