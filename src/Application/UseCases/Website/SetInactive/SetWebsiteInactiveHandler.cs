using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Website.SetInactive;

public record SetWebsiteInactiveCommand(Guid WebsiteId);

public sealed class SetWebsiteInactiveHandler : ICommandHandler<SetWebsiteInactiveCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public SetWebsiteInactiveHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SetWebsiteInactiveCommand setWebsiteInactiveCommand, CancellationToken cancellationToken)
    {
        var website = await _unitOfWork.WebsiteRepository.GetById(setWebsiteInactiveCommand.WebsiteId);
        if (website == null)
            return Errors.General.NotFound(nameof(Domain.Model.Website));

        website.SetInActive();

        await _unitOfWork.SaveAsync();
        return Result.Success();
    }
}