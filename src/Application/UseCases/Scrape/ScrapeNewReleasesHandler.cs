using Application.Ports.Persistence.Write;
using Application.UseCases.Base.CQS;
using Domain.Results;

namespace Application.UseCases.Scrape;

public record ScrapeNewReleasesCommand;

public sealed class ScrapeNewReleasesHandler : ICommandHandler<ScrapeNewReleasesCommand, Result>
{
    private readonly IMediaRepository _mediaRepository;

    public ScrapeNewReleasesHandler(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<Result> Handle(ScrapeNewReleasesCommand subscribeMedia, CancellationToken cancellationToken)
    {
        return await Task.FromResult(Result.Success());
    }
}