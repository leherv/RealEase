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
        var (pageIndex, pageSize) = mediaQuery;
        return await _availableMediaReadRepository.QueryAvailableMedia(new QueryParameters(pageIndex, pageSize));
    }
}