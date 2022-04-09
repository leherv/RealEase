namespace Application.Ports.Scraper;

public record ScrapeResult(string MediaName, int ReleaseNumber, int? SubReleaseNumber, string Url);