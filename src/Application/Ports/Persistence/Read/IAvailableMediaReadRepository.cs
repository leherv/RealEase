using Application.UseCases.Media.QueryAvailableMedia;

namespace Application.Ports.Persistence.Read;

public interface IAvailableMediaReadRepository
{
    Task<AvailableMedia> QueryAvailableMedia(QueryParameters queryParameters);
}

public record QueryParameters(int PageIndex, int PageSize)
{
    public int CalculateSkipForQuery()
    {
        return (PageIndex - 1) * PageSize;
    }
};