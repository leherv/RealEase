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
        var (pageIndex, pageSize, mediaNameSearchString, userQueryParameters) = mediaQuery;
        var queryParameters = new QueryParameters(
            pageIndex,
            pageSize,
            mediaNameSearchString,
            userQueryParameters != null
                ? new Ports.Persistence.Read.UserQueryParameters(
                    userQueryParameters.ExternalIdentifier,
                    userQueryParameters.SubscribeState
                )
                : null
        );

        return await _availableMediaReadRepository.QueryAvailableMedia(queryParameters);
    }
}