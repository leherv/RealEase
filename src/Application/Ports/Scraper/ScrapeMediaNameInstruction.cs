namespace Application.Ports.Scraper;

public record ScrapeMediaNameInstruction(string WebsiteName, string Url, string RelativeUrl);