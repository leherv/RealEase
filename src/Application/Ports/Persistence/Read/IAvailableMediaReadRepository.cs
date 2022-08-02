using Application.UseCases.Media.QueryAvailableMedia;

namespace Application.Ports.Persistence.Read;

public interface IAvailableMediaReadRepository
{
    Task<AvailableMedia> QueryAvailableMedia();
}