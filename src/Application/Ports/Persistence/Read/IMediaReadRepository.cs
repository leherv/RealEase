using Application.UseCases.Media;

namespace Application.Ports.Persistence.Read;

public interface IMediaReadRepository
{
    Task<AvailableMedia> QueryAvailableMedia();
}