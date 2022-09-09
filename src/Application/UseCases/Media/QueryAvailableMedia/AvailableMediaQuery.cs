using Application.Ports.Persistence.Read;

namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMediaQuery(
    int PageIndex,
    int PageSize,
    string? MediaName = null,
    UserQueryParameters? UserQueryParameters = null,
    SortBy? SortBy = null
);

public record UserQueryParameters(string ExternalIdentifier, SubscribeState SubscribeState = SubscribeState.All);

public record SortBy(SortColumn SortColumn, SortDirection SortDirection);

public enum SortColumn
{
    MediaName,
    SubscribeState
}

public enum SortDirection
{
    Asc,
    Desc
}