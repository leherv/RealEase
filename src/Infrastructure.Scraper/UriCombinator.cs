namespace Infrastructure.Scraper;

public static class UriCombinator
{
    public static string Combine(string uri1, string uri2)
    {
        uri1 = uri1.TrimEnd('/');
        uri2 = uri2.TrimStart('/');
        return $"{uri1}/{uri2}";
    }
}