using Application.Ports.Persistence.Read;
using Application.UseCases.Website.QueryAvailableWebsites;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Read;

public class WebsiteReadRepository : IWebsiteReadRepository
{
    private readonly DatabaseContext _databaseContext;

    public WebsiteReadRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<AvailableWebsites> QueryAvailableWebsites()
    {
        var websites = await _databaseContext.Websites
            .Where(website => website.Active)
            .Select(website => new AvailableWebsite(website.Id, website.Name, website.Url.Value))
            .ToListAsync();

        return new AvailableWebsites(websites);
    }
}