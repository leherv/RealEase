using Application.UseCases.Media.QueryAvailableMedia;

namespace Application.Ports.Persistence.Read;

public interface IAvailableMediaReadRepository
{
    Task<AvailableMedia> QueryAvailableMedia(QueryParameters queryParameters);
}

public enum SubscribeState
{
    All,
    Subscribed,
    Unsubscribed
}

public record QueryParameters(
    int PageIndex,
    int PageSize,
    string? MediaNameSearchString = null,
    UserQueryParameters? UserQueryParameters = null
)
{
    public int CalculateSkipForQuery()
    {
        return (PageIndex - 1) * PageSize;
    }

    public bool HasMediaNameSearchString => !string.IsNullOrEmpty(MediaNameSearchString);

    public bool SubscribeStateFilterActive => UserQueryParameters?.SubscribeState != null;
}

public record UserQueryParameters(string ExternalIdentifier, SubscribeState SubscribeState = SubscribeState.All);