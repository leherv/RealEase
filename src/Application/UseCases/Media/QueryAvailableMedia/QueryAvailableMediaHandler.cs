using Application.Ports.Persistence.Read;
using Application.UseCases.Base;

namespace Application.UseCases.Media.QueryAvailableMedia;

public sealed class QueryAvailableMediaHandler : IQueryHandler<AvailableMediaQuery, AvailableMedia>
{
    private readonly IAvailableMediaReadRepository _availableMediaReadRepository;

    public QueryAvailableMediaHandler(IAvailableMediaReadRepository availableMediaReadRepository)
    {
        _availableMediaReadRepository = availableMediaReadRepository;
    }

    public async Task<AvailableMedia> Handle(AvailableMediaQuery mediaQuery, CancellationToken cancellationToken)
    {
        var (pageIndex, pageSize, mediaNameSearchString, userQueryParameters, sortBy) = mediaQuery;
        var queryParameters = new QueryParameters(
            pageIndex,
            pageSize,
            mediaNameSearchString,
            userQueryParameters != null
                ? new Ports.Persistence.Read.UserQueryParameters(
                    userQueryParameters.ExternalIdentifier,
                    userQueryParameters.SubscribeState
                )
                : null,
            sortBy != null 
                ? new Ports.Persistence.Read.SortBy(
                    sortBy.SortColumn == SortColumn.MediaName 
                        ? Ports.Persistence.Read.SortColumn.MediaName
                        : Ports.Persistence.Read.SortColumn.SubscribeState,
                    sortBy.SortDirection == SortDirection.Desc
                        ? Ports.Persistence.Read.SortDirection.Desc
                        : Ports.Persistence.Read.SortDirection.Asc
                )
                : new Ports.Persistence.Read.SortBy(
                    Ports.Persistence.Read.SortColumn.MediaName,
                    Ports.Persistence.Read.SortDirection.Asc
                )
        );

        return await _availableMediaReadRepository.QueryAvailableMedia(queryParameters);
    }
}