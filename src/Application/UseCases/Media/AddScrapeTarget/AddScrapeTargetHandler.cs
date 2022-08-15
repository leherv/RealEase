using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;

namespace Application.UseCases.Media.AddScrapeTarget;

public record AddScrapeTargetCommand(string MediaName, string WebsiteName, string RelativeUrl);

public sealed class AddScrapeTargetHandler : ICommandHandler<AddScrapeTargetCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScraper _scraper;
    private readonly IMediaNameScraper _mediaNameScraper;

    public AddScrapeTargetHandler(
        IUnitOfWork unitOfWork,
        IScraper scraper,
        IMediaNameScraper mediaNameScraper
    )
    {
        _unitOfWork = unitOfWork;
        _scraper = scraper;
        _mediaNameScraper = mediaNameScraper;
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

        var resourceUrl = ResourceUrl.Create(website.Url, relativeUrlResult.Value);
        var scrapeInstruction = new ScrapeInstruction(
            media.Name,
            website.Name,
            website.Url.Value,
            resourceUrl.Value
        );
        var scrapeResult = await _scraper.Scrape(scrapeInstruction);
        if (scrapeResult.IsFailure)
            return scrapeResult.Error;
        
        var scrapeMediaNameInstruction = new ScrapeMediaNameInstruction(
            website.Name,
            resourceUrl.Value
        );
        var scrapeMediaNameResult = await _mediaNameScraper.ScrapeMediaName(scrapeMediaNameInstruction);
        if (scrapeMediaNameResult.IsFailure)
            return scrapeMediaNameResult.Error;

        var scrapedMediaName = scrapeMediaNameResult.Value.MediaName;
        var addScrapeTargetResult = media.AddScrapeTarget(scrapeTargetToAdd, scrapedMediaName);
        if (addScrapeTargetResult.IsFailure)
            return addScrapeTargetResult.Error;

        await _unitOfWork.SaveAsync();

        return Result.Success();
    }
}