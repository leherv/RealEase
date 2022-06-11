
namespace Infrastructure.Scraper.Base;

internal static class MediaNameScrapeStrategyFactory
{
    internal static IMediaNameScrapeStrategy Create(string websiteName)
    {
        return websiteName.ToLower() switch
        {
            "earlymanga" => new EarlyManga.MediaNameScrapeStrategy(),
            "manganato" => new Manganato.MediaNameScrapeStrategy(),
            "tapas" => new Tapas.MediaNameScrapeStrategy(),
            "mangapill" => new MangaPill.MediaNameScrapeStrategy(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}