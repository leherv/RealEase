using Application.UseCases.Website;

namespace Application.Ports.Persistence.Read;

public interface IWebsiteReadRepository
{
    Task<AvailableWebsites> QueryAvailableWebsites();
}