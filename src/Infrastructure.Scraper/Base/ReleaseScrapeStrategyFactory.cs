namespace Infrastructure.Scraper.Base;

internal static class ReleaseScrapeStrategyFactory
{
    internal static IReleaseScrapeStrategy Create(string websiteName)
    {
        return websiteName.ToLower() switch
        {
            "earlymanga" => new EarlyManga.ReleaseScrapeStrategy(),
            "manganato" => new Manganato.ReleaseScrapeStrategy(),
            "tapas" => new Tapas.ReleaseScrapeStrategy(),
            "mangapill" => new MangaPill.ReleaseScrapeStrategy(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}