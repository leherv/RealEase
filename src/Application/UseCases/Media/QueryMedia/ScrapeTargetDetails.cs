namespace Application.UseCases.Media.QueryMedia;

public record ScrapeTargetDetails(Guid WebsiteId, string WebsiteName, string WebsiteUrl, string ScrapeTargetUrl);