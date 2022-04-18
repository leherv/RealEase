using Domain.Model;

namespace Application.Ports.Persistence.Write;

public interface IMediaRepository
{
    Task<Media?> GetByName(string mediaName);
    Task<IReadOnlyCollection<Media>> GetAll();
}