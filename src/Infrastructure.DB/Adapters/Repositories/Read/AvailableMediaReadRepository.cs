using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryAvailableMedia;
using Domain.Model;
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
        var mediaQuery = _databaseContext.Media;
        
        if(queryParameters.HasMediaNameSearchString)
            mediaQuery = FilterMediaName(mediaQuery, queryParameters.MediaNameSearchString);
        
        var totalCount = await mediaQuery.CountAsync();

        mediaQuery = HandlePagination(mediaQuery, queryParameters);
        
        var media = await mediaQuery
            .OrderBy(media => media.Name)
            .Select(media => new MediaInformation(media.Id, media.Name))
            .ToListAsync();
        
        return new AvailableMedia(media, totalCount);
    }

    private static IQueryable<Media> FilterMediaName(IQueryable<Media> query, string mediaNameSearchString)
    {
        return query
            .Where(media => media.Name.ToLower().Contains(mediaNameSearchString.ToLower()));
    }

    private static IQueryable<Media> HandlePagination(IQueryable<Media> query, QueryParameters queryParameters)
    {
        return query
            .Skip(queryParameters.CalculateSkipForQuery())
            .Take(queryParameters.PageSize);
    }
}