using Application.Ports.Persistence.Read;
using Application.UseCases.Base;
using Domain.Results;

namespace Application.UseCases.Media.QueryScrapeTargets;

public record ScrapeTargetsQuery(string MediaName);

public sealed class QueryScrapeTargetsHandler : IQueryHandler<ScrapeTargetsQuery, Result<ScrapeTargets>>
{
    private readonly IScrapeTargetReadRepository _scrapeTargetReadRepository;

    public QueryScrapeTargetsHandler(IScrapeTargetReadRepository scrapeTargetReadRepository)
    {
        _scrapeTargetReadRepository = scrapeTargetReadRepository;
    }

    public async Task<Result<ScrapeTargets>> Handle(ScrapeTargetsQuery mediaQuery, CancellationToken cancellationToken)
    {
        return await _scrapeTargetReadRepository.QueryScrapeTargetsFor(mediaQuery.MediaName);
    }
}