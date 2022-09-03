using Application.Ports.Persistence.Read;

namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMediaQuery(
    int PageIndex,
    int PageSize,
    string? MediaName = null,
    UserQueryParameters? UserQueryParameters = null
);

public record UserQueryParameters(string ExternalIdentifier, SubscribeState SubscribeState = SubscribeState.All);