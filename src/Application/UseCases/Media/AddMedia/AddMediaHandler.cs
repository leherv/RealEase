using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;

namespace Application.UseCases.Media.AddMedia;

public record AddMediaCommand(string WebsiteName, string RelativeUrl);

public class AddMediaHandler : ICommandHandler<AddMediaCommand, Result>
{
    private readonly IMediaNameScraper _mediaNameScraper;
    private readonly IScraper _scraper;
    private readonly IUnitOfWork _unitOfWork;

    public AddMediaHandler(
        IMediaNameScraper mediaNameScraper,
        IScraper scraper,
        IUnitOfWork unitOfWork
    )
    {
        _mediaNameScraper = mediaNameScraper;
        _scraper = scraper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddMediaCommand scrapeNewReleasesCommand, CancellationToken cancellationToken)
    {
        var (websiteName, relativeUrl) = scrapeNewReleasesCommand;
        
        var website = await _unitOfWork.WebsiteRepository.GetByName(websiteName);
        if (website == null)
            return Errors.General.NotFound(nameof(Domain.Model.Website));

        var media = await _unitOfWork.MediaRepository.GetByUri(website, relativeUrl);
        if (media != null)
            return Errors.Media.MediaWithScrapeTargetExistsError(media.Name);

        var scrapeMediaNameInstruction = new ScrapeMediaNameInstruction(
            website.Name,
            website.Url,
            relativeUrl
        );
        var scrapeMediaNameResult = await _mediaNameScraper.ScrapeMediaName(scrapeMediaNameInstruction);
        if (scrapeMediaNameResult.IsFailure)
            return scrapeMediaNameResult.Error;
        media = await _unitOfWork.MediaRepository.GetByName(scrapeMediaNameResult.Value.MediaName);
        if (media != null)
            return Errors.Media.MediaWithNameExistsError(media.Name);

        var scrapeInstruction = new ScrapeInstruction(
            scrapeMediaNameResult.Value.MediaName,
            website.Name,
            website.Url,
            relativeUrl
        );
        var scrapeResult = await _scraper.Scrape(scrapeInstruction);
        if (scrapeResult.IsFailure)
            return scrapeResult.Error;

        var mediaResult = CreateScrapableMedia(
            scrapeMediaNameResult.Value.MediaName,
            relativeUrl,
            website
        );
        if (mediaResult.IsFailure)
            return mediaResult.Error;

        await _unitOfWork.MediaRepository.AddMedia(mediaResult.Value);
        await _unitOfWork.SaveAsync();

        return Result.Success();
    }

    private static Result<Domain.Model.Media> CreateScrapableMedia(
        string mediaName,
        string relativePath,
        Domain.Model.Website website
    )
    {
        var scrapeTarget = ScrapeTarget.Create(
            Guid.NewGuid(),
            website,
            relativePath
        );
        if (scrapeTarget.IsFailure)
            return scrapeTarget.Error;
        
        var media = Domain.Model.Media.Create(
            Guid.NewGuid(),
            mediaName,
            scrapeTarget.Value
        );
        return media;
    }
}