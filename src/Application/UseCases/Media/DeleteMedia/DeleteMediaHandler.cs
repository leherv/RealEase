using Application.Ports.Authorization;
using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Media.DeleteMedia;

public record DeleteMediaCommand(Guid MediaId, string ExternalIdentifier);

public sealed class DeleteMediaHandler : ICommandHandler<DeleteMediaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public DeleteMediaHandler(IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(DeleteMediaCommand deleteMediaCommand, CancellationToken cancellationToken)
    {
        if (!_authorizationService.IsAdmin(deleteMediaCommand.ExternalIdentifier))
        {
            return Errors.Authorization.AdminRightsMissing;
        }
        
        var mediaRepository = _unitOfWork.MediaRepository;
        var media = await mediaRepository.GetById(deleteMediaCommand.MediaId);
        if (media == null)
            return Errors.General.NotFound(nameof(Domain.Model.Media));
        
        mediaRepository.RemoveMedia(media);
        await _unitOfWork.SaveAsync();
        
        return Result.Success();
    }
}