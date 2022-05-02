using Application.UseCases.Media.QueryAvailableMedia;

namespace Application.Ports.Persistence.Read;

public interface IMediaReadRepository
{
    Task<AvailableMedia> QueryAvailableMedia();
}