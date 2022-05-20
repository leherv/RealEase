namespace Application.UseCases.Website.QueryAvailableWebsites;

public record AvailableWebsites(IReadOnlyCollection<AvailableWebsite> Websites);