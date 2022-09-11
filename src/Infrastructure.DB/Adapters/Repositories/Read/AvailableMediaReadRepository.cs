using Application.Ports.Persistence.Read;
using Application.UseCases.Media.QueryAvailableMedia;
using Domain.Model;
using Microsoft.EntityFrameworkCore;
using SortColumn = Application.Ports.Persistence.Read.SortColumn;
using SortDirection = Application.Ports.Persistence.Read.SortDirection;
using UserQueryParameters = Application.Ports.Persistence.Read.UserQueryParameters;

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

        if (queryParameters.SubscribeStateFilterActive)
            mediaQuery = await FilterSubscribeState(mediaQuery, queryParameters.UserQueryParameters);

        if (queryParameters.HasMediaNameSearchString)
            mediaQuery = FilterMediaName(mediaQuery, queryParameters.MediaNameSearchString);

        var totalCount = await mediaQuery.CountAsync();
        
        mediaQuery = await HandleOrdering(mediaQuery, queryParameters);

        mediaQuery = HandlePagination(mediaQuery, queryParameters);
        
        var media = await mediaQuery
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

    private async Task<IQueryable<Media>> FilterSubscribeState(
        IQueryable<Media> query,
        UserQueryParameters userQueryParameters
    )
    {
        var subscribedToMediaIds = await SubscribedToMediaIdsFor(userQueryParameters.ExternalIdentifier);

        return userQueryParameters.SubscribeState == SubscribeState.Subscribed 
            ? query.Where(media => subscribedToMediaIds.Contains(media.Id)) 
            : query.Where(media => !subscribedToMediaIds.Contains(media.Id));
    }

    private async Task<IReadOnlyCollection<Guid>> SubscribedToMediaIdsFor(string externalIdentifier)
    {
        var subscriber = await _databaseContext.Subscribers
            .Include(subscriber => subscriber.Subscriptions)
            .Where(subscriber => subscriber.ExternalIdentifier == externalIdentifier)
            .SingleAsync();

        return subscriber.SubscribedToMediaIds;
    }

    private async Task<IQueryable<Media>> HandleOrdering(IQueryable<Media> query, QueryParameters queryParameters)
    {
        var (sortColumn, sortDirection) = queryParameters.SortBy;
        if (sortColumn == SortColumn.MediaName)
        {
            return sortDirection == SortDirection.Asc
                ? query.OrderBy(media => media.Name)
                : query.OrderByDescending(media => media.Name);
        }

        var subscribedToMediaIds = await SubscribedToMediaIdsFor(queryParameters.UserQueryParameters.ExternalIdentifier);
        return sortDirection == SortDirection.Asc
            ? query.OrderBy(media => subscribedToMediaIds.Contains(media.Id)).ThenBy(media => media.Name)
            : query.OrderByDescending(media => subscribedToMediaIds.Contains(media.Id)).ThenBy(media => media.Name);
    }
}