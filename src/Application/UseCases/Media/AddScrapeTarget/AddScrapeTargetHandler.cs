using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;

namespace Application.UseCases.Media.AddScrapeTarget;

public record AddScrapeTargetCommand(string MediaName, string WebsiteName, string RelativeUrl);

public class AddScrapeTargetHandler : ICommandHandler<AddScrapeTargetCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScraper _scraper;

    public AddScrapeTargetHandler(IUnitOfWork unitOfWork, IScraper scraper)
    {
        _unitOfWork = unitOfWork;
        _scraper = scraper;
    }

    public async Task<Result> Handle(AddScrapeTargetCommand scrapeNewReleasesCommand, CancellationToken cancellationToken)
    {
        var media = await _unitOfWork.MediaRepository.GetByName(scrapeNewReleasesCommand.MediaName);
        if (media == null)
            return Errors.General.NotFound(nameof(Domain.Model.Media));

        var website = await _unitOfWork.WebsiteRepository.GetByName(scrapeNewReleasesCommand.WebsiteName);
        if (website == null)
            return Errors.General.NotFound(nameof(Domain.Model.Website));

        var relativeUrlResult = RelativeUrl.Create(scrapeNewReleasesCommand.RelativeUrl);
        if (relativeUrlResult.IsFailure)
            return relativeUrlResult.Error;

        var scrapeTargetToAddResult = ScrapeTarget.Create(Guid.NewGuid(), website, relativeUrlResult.Value);
        if (scrapeTargetToAddResult.IsFailure)
            return scrapeTargetToAddResult;
        var scrapeTargetToAdd = scrapeTargetToAddResult.Value;
        
        if(media.ScrapeTargetAlreadyConfigured(scrapeTargetToAdd))
            return Errors.Media.ScrapeTargetExistsError(media.Name);
        
        var scrapeInstruction = new ScrapeInstruction(
            media.Name,
            website.Name,
            website.Url.Value,
            scrapeNewReleasesCommand.RelativeUrl
        );
        var scrapeResult = await _scraper.Scrape(scrapeInstruction);
        if (scrapeResult.IsFailure)
            return scrapeResult.Error;

        var addScrapeTargetResult = media.AddScrapeTarget(scrapeTargetToAdd);
        if (addScrapeTargetResult.IsFailure)
            return scrapeResult.Error;

        await _unitOfWork.SaveAsync();

        return Result.Success();
    }
}