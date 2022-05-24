using Domain.Model;

namespace Application.Ports.Persistence.Write;

public interface IMediaRepository
{
    Task<Media?> GetByName(string mediaName);
    Task<Media?> GetByUri(Guid websiteId, string relativeUrl);
    Task<IReadOnlyCollection<Media>> GetAll();
    Task AddMedia(Media media);
}