using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Media.DeleteMedia;

public record DeleteMediaCommand(Guid MediaId);

public sealed class DeleteMediaHandler : ICommandHandler<DeleteMediaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMediaHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteMediaCommand deleteMediaCommand, CancellationToken cancellationToken)
    {
        var mediaRepository = _unitOfWork.MediaRepository;
        var media = await mediaRepository.GetById(deleteMediaCommand.MediaId);
        if (media == null)
            return Errors.General.NotFound(nameof(Domain.Model.Media));
        
        mediaRepository.RemoveMedia(media);
        await _unitOfWork.SaveAsync();
        
        return Result.Success();
    }
}