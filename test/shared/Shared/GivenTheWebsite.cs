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
        EarlyManga = Create(Guid.NewGuid(), "https://earlymng.org/", "earlymanga").Value;
        Manganato = Create(Guid.NewGuid(), "https://manganato.com/", "manganato").Value;
        
        Websites = new List<Website>
        {
            EarlyManga,
            Manganato
        };
    }

    public static Result<Website> Create(
        Guid? id = null,
        string? url = null,
        string? websiteName = null
    )
    {
        return Website.Create(
            id ?? Guid.NewGuid(),
            url ?? "https://earlymng.org/",
            websiteName ?? "earlymanga"
        );
    }
}