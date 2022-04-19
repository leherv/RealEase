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

    public ScrapeNewReleasesHandler(
        IScraper scraper,
        IUnitOfWork unitOfWork
    )
    {
        _scraper = scraper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ScrapeNewReleasesCommand scrapeNewReleasesCommand, CancellationToken cancellationToken)
    {
        var mediaToScrape = await GetMediaToScrape(scrapeNewReleasesCommand.MediaToScrape);

        foreach (var mediaName in mediaToScrape)
        {
            var media = await _unitOfWork.MediaRepository.GetByName(mediaName);

            if (media == null)
                return Errors.General.NotFound(nameof(Domain.Model.Media));
            
            var scrapeInstruction = new ScrapeInstruction("test", mediaName.ToLower());
            var scrapeResult = await _scraper.Scrape(scrapeInstruction);

            if (scrapeResult.IsSuccess)
            {
                var scrapedMediaRelease = scrapeResult.Value;
                var releaseResult = scrapedMediaRelease.ToDomain();
                if (releaseResult.IsSuccess)
                {
                    var publishResult = media.PublishNewRelease(releaseResult.Value);
                    // maybe log later
                }
            }
        }

        await _unitOfWork.SaveAsync();
        return Result.Success();
    }

    public async Task<IReadOnlyCollection<string>> GetMediaToScrape(IEnumerable<string> mediaToScrape)
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