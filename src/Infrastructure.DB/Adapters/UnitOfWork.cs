using Application.Ports.Persistence.Write;

namespace Infrastructure.DB.Adapters;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _databaseContext;
    public IMediaRepository MediaRepository { get; }
    public ISubscriberRepository SubscriberRepository { get; }
    public IWebsiteRepository WebsiteRepository { get; }

    public UnitOfWork(
        DatabaseContext databaseContext,
        IMediaRepository mediaRepository,
        ISubscriberRepository subscriberRepository,
        IWebsiteRepository websiteRepository
    )
    {
        _databaseContext = databaseContext;
        MediaRepository = mediaRepository;
        SubscriberRepository = subscriberRepository;
        WebsiteRepository = websiteRepository;
    }
    
    public async Task<int> SaveAsync()
    {
        return await _databaseContext.SaveChangesAsync();
    }
}