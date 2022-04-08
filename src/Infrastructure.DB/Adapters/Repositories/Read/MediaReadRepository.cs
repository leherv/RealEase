using Application.Ports.Persistence.Read;
using Application.UseCases.Media;
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
        var mediaNames = await _databaseContext.Media
            .Select(media => media.Name)
            .ToListAsync();

        return new AvailableMedia(mediaNames);
    }
}