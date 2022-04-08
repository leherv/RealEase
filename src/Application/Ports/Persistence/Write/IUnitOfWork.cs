namespace Application.Ports.Persistence.Write;

public interface IUnitOfWork
{
    IMediaRepository MediaRepository { get; }
    ISubscriberRepository SubscriberRepository { get; }
    Task<int> SaveAsync();
    
}