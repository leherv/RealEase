using Domain.Model;

namespace Application.Ports.Persistence.Write;

public interface IWebsiteRepository
{
    Task<Website?> GetByName(string websiteName);
    Task<Website?> GetById(Guid id);
}