﻿using Application.Ports.Persistence.Read;
using Application.UseCases.Website;
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
            .Select(website => new AvailableWebsite(website.Name, website.Url))
            .ToListAsync();

        return new AvailableWebsites(websites);
    }
}