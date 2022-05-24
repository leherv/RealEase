using Application.Ports.General;
using Application.Ports.Persistence.Write;
using Application.Ports.Scraper;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;

namespace Application.UseCases.Scrape;

// if empty all will be scraped
public record ScrapeNewReleasesCommand(IEnumerable<string> MediaToScrape);

public sealed class ScrapeNewReleasesHandler : ICommandHandler<ScrapeNewReleasesCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScraper _scraper;
    private readonly IApplicationLogger _applicationLogger;
    private readonly ITimeProvider _timeProvider;

    public ScrapeNewReleasesHandler(
        IScraper scraper,
        IUnitOfWork unitOfWork,
        IApplicationLogger applicationLogger,
        ITimeProvider timeProvider
    )
    {
        _scraper = scraper;
        _unitOfWork = unitOfWork;
        _applicationLogger = applicationLogger;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(ScrapeNewReleasesCommand scrapeNewReleasesCommand,
        CancellationToken cancellationToken)
    {
        var mediaNamesToScrape = await GetMediaNamesToScrape(scrapeNewReleasesCommand.MediaToScrape);
        var mediaToScrapeResult = await GetMediaToScrape(mediaNamesToScrape);
        if (mediaToScrapeResult.IsFailure)
            return mediaToScrapeResult.Error;

        foreach (var media in mediaToScrapeResult.Value)
        {
            var scrapeInstructions = await PrepareScrapeInstructionsForMedia(media);
            var scrapedMediaReleasesResults = await ScrapeForReleases(scrapeInstructions);
            LogFailingScrapes(scrapedMediaReleasesResults);
            var scrapedMediaReleases = FilterSuccessfulScrapes(scrapedMediaReleasesResults);
            var releasesResults = TransformToReleases(scrapedMediaReleases);
            LogFailingCreations(releasesResults);
            var releases = FilterSuccessfulReleases(releasesResults);
            var newestRelease = SelectNewestRelease(releases);

            if (newestRelease == null)
            {
                _applicationLogger.LogWarning($"No scrapeTarget delivered a successful result while scraping media with name {media.Name}");
                continue;
            }

            if (media.ReleaseCanBePublished(newestRelease))
            {
                media.PublishNewRelease(newestRelease);
            }
            else
            {
                _applicationLogger.LogWarning($"Newest Release is not newer for media with name {media.Name}");
            }
        }

        await _unitOfWork.SaveAsync();
        return Result.Success();
    }
    
    private async Task<IReadOnlyCollection<Result<ScrapedMediaRelease>>> ScrapeForReleases(
        IEnumerable<ScrapeInstruction> scrapeInstructions)
    {
        var scrapeTasks = scrapeInstructions.Select(scrapeInstruction => _scraper.Scrape(scrapeInstruction));
        return await Task.WhenAll(scrapeTasks);
    }

    private void LogFailingScrapes(IEnumerable<Result<ScrapedMediaRelease>> scrapedMediaReleasesResults)
    {
        foreach (var scrapedMediaReleaseResult in scrapedMediaReleasesResults)
        {
            if (scrapedMediaReleaseResult.IsFailure)
                _applicationLogger.LogWarning(scrapedMediaReleaseResult.Error.ToString());
        }
    }

    private static IEnumerable<ScrapedMediaRelease> FilterSuccessfulScrapes(
        IEnumerable<Result<ScrapedMediaRelease>> scrapedMediaReleasesResults)
    {
        return scrapedMediaReleasesResults
            .Where(scrapedMediaReleaseResult => scrapedMediaReleaseResult.IsSuccess)
            .Select(scrapedMediaReleaseResult => scrapedMediaReleaseResult.Value);
    }
    
    private IReadOnlyCollection<Result<Release>> TransformToReleases(IEnumerable<ScrapedMediaRelease> scrapedMediaReleases)
    {
        return scrapedMediaReleases
            .Select(scrapedMediaRelease => scrapedMediaRelease.ToDomain(_timeProvider.UtcNow))
            .ToList();
    }
    
    private void LogFailingCreations(IEnumerable<Result<Release>> releaseResults)
    {
        foreach (var releaseResult in releaseResults)
        {
            if (releaseResult.IsFailure)
                _applicationLogger.LogWarning(releaseResult.Error.ToString());
        }
    }
    
    private static IReadOnlyCollection<Release> FilterSuccessfulReleases(
        IEnumerable<Result<Release>> releaseResults)
    {
        return releaseResults
            .Where(releaseResult => releaseResult.IsSuccess)
            .Select(releaseResult => releaseResult.Value)
            .ToList();
    }

    private static Release? SelectNewestRelease(IEnumerable<Release> releases)
    {
        return releases
            .MaxBy(release => release.ReleaseNumber);
    }

    private async Task<IEnumerable<ScrapeInstruction>> PrepareScrapeInstructionsForMedia(
        Domain.Model.Media media)
    {
        var scrapeInstructions = new List<ScrapeInstruction>();
        foreach (var mediaScrapeTarget in media.ScrapeTargets)
        {
            var website = await _unitOfWork.WebsiteRepository.GetById(mediaScrapeTarget.WebsiteId);
            if (website == null)
                continue;
            var scrapeInstruction = PrepareScrapeInstruction(mediaScrapeTarget, media, website);
            scrapeInstructions.Add(scrapeInstruction);
        }

        return scrapeInstructions;
    }

    private static ScrapeInstruction PrepareScrapeInstruction(
        ScrapeTarget scrapeTarget,
        Domain.Model.Media media,
        Domain.Model.Website website
    )
    {
        var resourceUrl = ResourceUrl.Create(website.Url, scrapeTarget.RelativeUrl);
        return new ScrapeInstruction(
            media.Name,
            website.Name,
            website.Url.Value,
            resourceUrl.Value
        );
    }

    private async Task<Result<IReadOnlyCollection<Domain.Model.Media>>> GetMediaToScrape(IEnumerable<string> mediaNames)
    {
        var mediaToScrape = new List<Domain.Model.Media>();
        foreach (var mediaName in mediaNames)
        {
            var media = await _unitOfWork.MediaRepository.GetByName(mediaName);
            if (media == null)
                return Errors.General.NotFound(nameof(Domain.Model.Media));
            mediaToScrape.Add(media);
        }

        return Result<IReadOnlyCollection<Domain.Model.Media>>.Success(mediaToScrape);
    }

    private async Task<IReadOnlyCollection<string>> GetMediaNamesToScrape(IEnumerable<string> mediaToScrape)
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