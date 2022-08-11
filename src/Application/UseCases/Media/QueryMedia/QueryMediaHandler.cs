using Application.Ports.Persistence.Read;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Media.QueryMedia;

public record MediaQuery(Guid Id);

public sealed class QueryMediaHandler : IQueryHandler<MediaQuery, Result<MediaDetails>>
{
    private readonly IMediaReadRepository _mediaReadRepository;

    public QueryMediaHandler(IMediaReadRepository mediaReadRepository)
    {
        _mediaReadRepository = mediaReadRepository;
    }

    public async Task<Result<MediaDetails>> Handle(MediaQuery mediaQuery, CancellationToken cancellationToken)
    {
        var mediaDetails = await _mediaReadRepository.QueryById(mediaQuery.Id);
        
        return mediaDetails ?? Result<MediaDetails>.Failure(Errors.General.NotFound(nameof(Domain.Model.Media)));
    }
}