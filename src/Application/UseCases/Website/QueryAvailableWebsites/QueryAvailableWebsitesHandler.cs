using Application.Ports.Persistence.Read;
using Application.UseCases.Base;

namespace Application.UseCases.Website.QueryAvailableWebsites;

public record AvailableWebsitesQuery;

public sealed class QueryAvailableWebsitesHandler : IQueryHandler<AvailableWebsitesQuery, AvailableWebsites>
{
    private readonly IWebsiteReadRepository _websiteReadRepository;

    public QueryAvailableWebsitesHandler(IWebsiteReadRepository websiteReadRepository)
    {
        _websiteReadRepository = websiteReadRepository;
    }

    public async Task<AvailableWebsites> Handle(AvailableWebsitesQuery mediaQuery, CancellationToken cancellationToken)
    {
        return await _websiteReadRepository.QueryAvailableWebsites();
    }
}