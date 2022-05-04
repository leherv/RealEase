namespace Application.Ports.Scraper;

public record ScrapeInstruction(string MediaName, string WebsiteName, string Url, string RelativeUrl);