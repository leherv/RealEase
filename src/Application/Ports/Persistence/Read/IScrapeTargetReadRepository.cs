using Application.UseCases.Media.QueryScrapeTargets;
using Domain.Results;

namespace Application.Ports.Persistence.Read;

public interface IScrapeTargetReadRepository
{
    Task<Result<ScrapeTargets>> QueryScrapeTargetsFor(string mediaName);
}