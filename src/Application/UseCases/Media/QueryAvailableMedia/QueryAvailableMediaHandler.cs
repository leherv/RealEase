using Application.Ports.Persistence.Read;
using Application.UseCases.Base;

namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMediaQuery;

public sealed class QueryAvailableMediaHandler : IQueryHandler<AvailableMediaQuery, AvailableMedia>
{
    private readonly IMediaReadRepository _mediaReadRepository;

    public QueryAvailableMediaHandler(IMediaReadRepository mediaReadRepository)
    {
        _mediaReadRepository = mediaReadRepository;
    }

    public async Task<AvailableMedia> Handle(AvailableMediaQuery availableMediaQuery, CancellationToken cancellationToken)
    {
        return await _mediaReadRepository.QueryAvailableMedia();
    }
}