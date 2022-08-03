using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheWebsite
{
    public List<Website> ActiveWebsites { get; }
    public List<Website> InActiveWebsites { get; }
    public List<Website> Websites => ActiveWebsites.Concat(InActiveWebsites).ToList();
    public Website EarlyManga { get; }
    public Website Manganato { get; }
    public Website Tapas { get; }
    public Website MangaPill { get; }
    public Website MangaWalker { get; }

    public GivenTheWebsite()
    {
        EarlyManga = Create(
            Guid.NewGuid(),
            WebsiteUrl.Create("https://earlymng.org/").Value,
            "earlymanga"
        ).Value;
        
        Manganato = Create(
            Guid.NewGuid(),
            WebsiteUrl.Create("https://readmanganato.com/").Value,
            "manganato"
        ).Value;
        
        Tapas = Create(
            Guid.NewGuid(),
            WebsiteUrl.Create("https://tapas.io/").Value,
            "tapas"
        ).Value;
        
        MangaPill = Create(
            Guid.NewGuid(),
            WebsiteUrl.Create("https://mangapill.com/").Value,
            "mangapill"
        ).Value;
        
        MangaWalker = Create(
            Guid.NewGuid(),
            WebsiteUrl.Create("https://comic-walker.com/").Value,
            "mangawalker"
        ).Value;
        MangaWalker.SetInActive();

        ActiveWebsites = new List<Website>
        {
            EarlyManga,
            Manganato,
            Tapas,
            MangaPill
        };

        InActiveWebsites = new List<Website>
        {
            MangaWalker
        };
    }

    public static Result<Website> Create(
        Guid? id = null,
        WebsiteUrl? url = null,
        string? websiteName = null
    )
    {
        return Website.Create(
            id ?? Guid.NewGuid(),
            url ?? WebsiteUrl.Create("https://earlymng.org/").Value,
            websiteName ?? "earlymanga"
        );
    }
}