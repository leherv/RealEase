using Application.Ports.Authorization;
using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Model;
using Domain.Results;

namespace Application.UseCases.Media.DeleteScrapeTarget;

public record DeleteScrapeTargetCommand(Guid MediaId, Guid ScrapeTargetId, string ExternalIdentifier);

public sealed class DeleteScrapeTargetHandler : ICommandHandler<DeleteScrapeTargetCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public DeleteScrapeTargetHandler(IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteScrapeTargetCommand deleteScrapeTargetCommand, CancellationToken cancellationToken)
    {
        if (!_authorizationService.IsAdmin(deleteScrapeTargetCommand.ExternalIdentifier))
        {
            return Errors.Authorization.AdminRightsMissing;
        }
        
        var media = await _unitOfWork.MediaRepository.GetById(deleteScrapeTargetCommand.MediaId);
        if (media == null)
            return Errors.General.NotFound(nameof(Domain.Model.Media));

        var result = media.RemoveScrapeTarget(deleteScrapeTargetCommand.ScrapeTargetId) 
            ? Result.Success() 
            : Result.Failure(Errors.General.NotFound(nameof(ScrapeTarget)));

        await _unitOfWork.SaveAsync();
        
        return result;
    }
}