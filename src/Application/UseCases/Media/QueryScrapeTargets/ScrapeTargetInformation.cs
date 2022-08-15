namespace Application.UseCases.Media.QueryScrapeTargets;

public record ScrapeTargetInformation(Guid ScrapeTargetId, string WebsiteName, string WebsiteUrl, string RelativeUrl);