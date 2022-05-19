using Application.Ports.Persistence.Write;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Write;

public class WebsiteRepository : IWebsiteRepository
{
    private readonly DatabaseContext _databaseContext;

    public WebsiteRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Website?> GetByName(string websiteName)
    {
        return await _databaseContext.Websites
            .Where(website => website.Name.ToLower() == websiteName.ToLower())
            .FirstOrDefaultAsync();
    }

    public async Task<Website?> GetById(Guid id)
    {
        return await _databaseContext.Websites
            .Where(website => website.Id == id)
            .FirstOrDefaultAsync();
    }
}