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

    public async Task<AvailableMedia> QueryAvailableMedia(QueryParameters queryParameters)
    {
        var totalCount = await _databaseContext.Media.CountAsync();
        
        var media = await _databaseContext.Media
            .Skip(queryParameters.CalculateSkipForQuery())
            .Take(queryParameters.PageSize)
            .Select(media => new MediaInformation(media.Id, media.Name))
            .ToListAsync();

        return new AvailableMedia(media, totalCount);
    }
}