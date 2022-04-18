using Application.Ports.Persistence.Write;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Adapters.Repositories.Write;

public class MediaRepository : IMediaRepository
{
    private readonly DatabaseContext _databaseContext;

    public MediaRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    
    public async Task<Media?> GetByName(string mediaName)
    {
        return await _databaseContext.Media.SingleOrDefaultAsync(media => media.Name == mediaName);
    }

    public async Task<IReadOnlyCollection<Media>> GetAll()
    {
        return await _databaseContext.Media.ToListAsync();
    }
}