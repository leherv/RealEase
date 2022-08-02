using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryAvailableMedia;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class AvailableMediaReadRepository : IAvailableMediaReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public AvailableMediaReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<AvailableMedia> QueryAvailableMedia()
    {
        var media = await _databaseContext.Media
            .Select(media => new MediaInformation(media.Id, media.Name))
            .ToListAsync();

        return new AvailableMedia(media);
    }
}