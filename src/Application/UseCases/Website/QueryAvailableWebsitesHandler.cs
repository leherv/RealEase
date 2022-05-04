using Application.Ports.Persistence.Read;
using Application.UseCases.Base;

namespace Application.UseCases.Website;

public record AvailableWebsitesQuery;

public sealed class QueryAvailableWebsitesHandler : IQueryHandler<AvailableWebsitesQuery, AvailableWebsites>
{
    private readonly IWebsiteReadRepository _websiteReadRepository;

    public QueryAvailableWebsitesHandler(IWebsiteReadRepository websiteReadRepository)
    {
        _websiteReadRepository = websiteReadRepository;
    }

    public async Task<AvailableWebsites> Handle(AvailableWebsitesQuery query, CancellationToken cancellationToken)
    {
        return await _websiteReadRepository.QueryAvailableWebsites();
    }
}