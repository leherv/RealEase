using Application.UseCases.Website.QueryAvailableWebsites;

namespace Application.Ports.Persistence.Read;

public interface IWebsiteReadRepository
{
    Task<AvailableWebsites> QueryAvailableWebsites();
}