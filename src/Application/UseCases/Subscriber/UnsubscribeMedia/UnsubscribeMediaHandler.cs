using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Application.UseCases.Base.CQS;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Subscriber.UnsubscribeMedia;

public record UnsubscribeMediaCommand(string ExternalIdentifier, string MediaName);

public sealed class UnsubscribeMediaHandler : ICommandHandler<UnsubscribeMediaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UnsubscribeMediaHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UnsubscribeMediaCommand subscribeMediaCommand, CancellationToken cancellationToken)
    {
        var (externalIdentifier, mediaName) = subscribeMediaCommand;
        
        var subscriber = await _unitOfWork.SubscriberRepository.GetByExternalId(externalIdentifier);
        if (subscriber == null)
            return Errors.General.NotFound(nameof(Domain.Model.Subscriber)); 
                
        var media = await _unitOfWork.MediaRepository.GetByName(mediaName);
        if(media == null)
            return Result.Failure(Errors.General.NotFound(nameof(Domain.Model.Media))); 

        var result = subscriber.Unsubscribe(media);
        if (result.IsFailure)
            return result;

        await _unitOfWork.SaveAsync();
        
        return Result.Success();
    }
}