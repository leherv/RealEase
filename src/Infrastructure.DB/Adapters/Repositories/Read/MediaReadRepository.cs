using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryAvailableMedia;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class MediaReadRepository : IMediaReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public MediaReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<AvailableMedia> QueryAvailableMedia()
    {
        var media = await _databaseContext.Media
            .Select(media => new MediaInfo(media.Id, media.Name))
            .ToListAsync();

        return new AvailableMedia(media);
    }
}