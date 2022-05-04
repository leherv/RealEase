using Domain.Model;

namespace Application.Ports.Persistence.Write;

public interface IMediaRepository
{
    Task<Media?> GetByName(string mediaName);
    Task<Media?> GetByUri(Website website, string relativeUrl);
    Task<IReadOnlyCollection<Media>> GetAll();
    Task AddMedia(Media media);
}