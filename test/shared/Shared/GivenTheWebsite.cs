using Domain.Model;
using Domain.Results;

namespace Shared;

public class GivenTheWebsite
{
    public List<Website> Websites { get; }
    public Website EarlyManga { get; }
    public Website Manganato { get; }

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

        Websites = new List<Website>
        {
            EarlyManga,
            Manganato
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