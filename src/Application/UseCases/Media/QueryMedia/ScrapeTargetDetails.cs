namespace Application.UseCases.Media.QueryMedia;

public record ScrapeTargetDetails(Guid ScrapeTargetId, string WebsiteName, string WebsiteUrl, string ScrapeTargetUrl);