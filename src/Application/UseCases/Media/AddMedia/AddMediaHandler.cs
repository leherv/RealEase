using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Media.AddMedia;

public record AddMediaCommand(string WebsiteName);

public class AddMediaHandler : ICommandHandler<AddMediaCommand, Result>
{
    private readonly IWebsiteRepository _websiteRepository;

    public AddMediaHandler(IWebsiteRepository websiteRepository)
    {
        _websiteRepository = websiteRepository;
    }

    public async Task<Result> Handle(AddMediaCommand scrapeNewReleasesCommand, CancellationToken cancellationToken)
    {
        var website = await _websiteRepository.GetByName(scrapeNewReleasesCommand.WebsiteName);
        if (website == null)
            return Errors.General.NotFound(nameof(Domain.Model.Website));
        
        return Result.Success();
    }
}