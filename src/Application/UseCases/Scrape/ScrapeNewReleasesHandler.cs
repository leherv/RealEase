using Application.Ports.General;
using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Scrape;

// if empty all will be scraped
public record ScrapeNewReleasesCommand(IEnumerable<string> MediaToScrape);

public sealed class ScrapeNewReleasesHandler : ICommandHandler<ScrapeNewReleasesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScraper _scraper;
    private readonly IApplicationLogger _applicationLogger;

    public ScrapeNewReleasesHandler(
        IScraper scraper,
        IUnitOfWork unitOfWork,
        IApplicationLogger applicationLogger
    )
    {
        _scraper = scraper;
        _unitOfWork = unitOfWork;
        _applicationLogger = applicationLogger;
    }

    public async Task<Result> Handle(ScrapeNewReleasesCommand scrapeNewReleasesCommand, CancellationToken cancellationToken)
    {
        var mediaToScrape = await GetMediaToScrape(scrapeNewReleasesCommand.MediaToScrape);

        foreach (var mediaName in mediaToScrape)
        {
            var media = await _unitOfWork.MediaRepository.GetByName(mediaName);
            if (media == null)
                return Errors.General.NotFound(nameof(Domain.Model.Media));
            
            if(media.ScrapeTarget == null)
                continue;

            var website = await _unitOfWork.WebsiteRepository.GetById(media.ScrapeTarget.WebsiteId);
            if(website == null)
                continue;

            var scrapeInstruction = new ScrapeInstruction(
                media.Name,
                website.Name,
                website.Url,
                media.ScrapeTarget.RelativeUrl
            );
            var scrapeResult = await _scraper.Scrape(scrapeInstruction);

            if (scrapeResult.IsFailure)
            {
                _applicationLogger.LogWarning(scrapeResult.Error.ToString());
            }
            else
            {
                var scrapedMediaRelease = scrapeResult.Value;
                var releaseResult = scrapedMediaRelease.ToDomain();
                if (releaseResult.IsSuccess)
                    media.PublishNewRelease(releaseResult.Value);
            }
        }

        await _unitOfWork.SaveAsync();
        return Result.Success();
    }
    
    private async Task<IReadOnlyCollection<string>> GetMediaToScrape(IEnumerable<string> mediaToScrape)
    {
        var toScrape = mediaToScrape.ToList();
        if (toScrape.Any())
            return toScrape.ToList();
        
        var allMedia = await _unitOfWork.MediaRepository.GetAll();
        
        return allMedia
            .Select(media => media.Name)
            .ToList();
    }
}