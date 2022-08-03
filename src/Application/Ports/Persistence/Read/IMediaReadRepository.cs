using Application.UseCases.Media.QueryMedia;

namespace Application.Ports.Persistence.Read;

public interface IMediaReadRepository
{
    Task<MediaDetails?> QueryById(Guid id);
}