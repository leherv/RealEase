namespace Application.UseCases.Media.QueryAvailableMedia;

public record AvailableMediaQuery(int PageIndex, int PageSize, string? MediaName = null);