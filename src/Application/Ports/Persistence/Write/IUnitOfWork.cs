namespace Application.Ports.Persistence.Write;

public interface IUnitOfWork
{
    IMediaRepository MediaRepository { get; }
    ISubscriberRepository SubscriberRepository { get; }
    IWebsiteRepository WebsiteRepository { get; }
    Task<int> SaveAsync();
    
}