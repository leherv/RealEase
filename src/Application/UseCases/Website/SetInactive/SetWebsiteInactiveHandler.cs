using Application.Ports.Authorization;
using Application.Ports.Persistence.Write;
using Application.UseCases.Base;
using Domain.ApplicationErrors;
using Domain.Results;

namespace Application.UseCases.Website.SetInactive;

public record SetWebsiteInactiveCommand(Guid WebsiteId, string ExternalIdentifier);

public sealed class SetWebsiteInactiveHandler : ICommandHandler<SetWebsiteInactiveCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorizationService _authorizationService;

    public SetWebsiteInactiveHandler(IUnitOfWork unitOfWork, IAuthorizationService authorizationService)
    {
        _unitOfWork = unitOfWork;
        _authorizationService = authorizationService;
    }

    public async Task<Result> Handle(SetWebsiteInactiveCommand setWebsiteInactiveCommand, CancellationToken cancellationToken)
    {
        if (!_authorizationService.IsAdmin(setWebsiteInactiveCommand.ExternalIdentifier))
        {
            return Errors.Authorization.AdminRightsMissing;
        }
        
        var website = await _unitOfWork.WebsiteRepository.GetById(setWebsiteInactiveCommand.WebsiteId);
        if (website == null)
            return Errors.General.NotFound(nameof(Domain.Model.Website));

        website.SetInActive();

        await _unitOfWork.SaveAsync();
        return Result.Success();
    }
}