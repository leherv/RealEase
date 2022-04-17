using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Application.UseCases.Base.CQS;
using Domain.ApplicationErrors;
using Domain.Results;
using Domain.Results.Extensions;

namespace Application.UseCases.Subscriber.SubscribeMedia;

public record SubscribeMediaCommand(string ExternalIdentifier, string MediaName);

public sealed class SubscribeMediaHandler : ICommandHandler<SubscribeMediaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SubscribeMediaHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SubscribeMediaCommand subscribeMediaCommand, CancellationToken cancellationToken)
    {
        var (externalIdentifier, mediaName) = subscribeMediaCommand;

        var mediaResult = await GetMedia(mediaName)
            .ToResult(Errors.General.NotFound(nameof(Domain.Model.Media)));

        if (mediaResult.IsFailure)
            return mediaResult;

        var subscriberResult = await GetOrCreateSubscriber(externalIdentifier);
        if (subscriberResult.IsFailure)
            return subscriberResult;
        
        subscriberResult.Value.Subscribe(mediaResult.Value);
        
        await _unitOfWork.SaveAsync();
        
        return Result.Success();
    }

    private async Task<Domain.Model.Media?> GetMedia(string mediaName)
    {
        var media = await _unitOfWork.MediaRepository.GetByName(mediaName);
        return media;
    }

    private async Task<Result<Domain.Model.Subscriber>> GetOrCreateSubscriber(string externalIdentifier)
    {
        var subscriber = await _unitOfWork.SubscriberRepository.GetByExternalId(externalIdentifier);
        if (subscriber == null)
        {
            var subscriberResult = Domain.Model.Subscriber.Create(Guid.NewGuid(), externalIdentifier);
            subscriber = subscriberResult.Value;
            await _unitOfWork.SubscriberRepository.AddSubscriber(subscriber);
        }

        return subscriber;
    }
}