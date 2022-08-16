using Application.UseCases.Base;
using Application.UseCases.Website.QueryAvailableWebsites;
using Application.UseCases.Website.SetInactive;
using AspNetCoreHero.ToastNotification.Abstractions;
using Domain.ApplicationErrors;
using Domain.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RealEaseApp.Extensions;

namespace RealEaseApp.Pages;

public class Websites : PageModel
{
    public IReadOnlyCollection<WebsiteViewModel> WebsiteViewModels { get; private set; }

    private readonly IQueryDispatcher _queryDispatcher;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly ILogger<Websites> _logger;
    private readonly IToastifyService _toastifyService;

    public Websites(
        IQueryDispatcher queryDispatcher,
        ICommandDispatcher commandDispatcher,
        ILogger<Websites> logger,
        IToastifyService toastifyService
    )
    {
        _queryDispatcher = queryDispatcher;
        _commandDispatcher = commandDispatcher;
        _logger = logger;
        _toastifyService = toastifyService;
    }

    public async Task OnGet()
    {
        await SetupPage();
    }

    public async Task<IActionResult> OnPost(Guid websiteId)
    {
        var setWebsiteInactiveCommand = new SetWebsiteInactiveCommand(websiteId, User.GetExternalIdentifier());

        var setWebsiteInactiveResult =
            await _commandDispatcher.Dispatch<SetWebsiteInactiveCommand, Result>(setWebsiteInactiveCommand);
        
        if (setWebsiteInactiveResult.IsFailure)
        {
            _logger.LogError(setWebsiteInactiveResult.Error.ToString());
            _toastifyService.Error(BuildErrorMessage(setWebsiteInactiveResult));
        }

        await SetupPage();
        return Page();
    }

    private async Task SetupPage()
    {
        var availableWebsites =
            await _queryDispatcher.Dispatch<AvailableWebsitesQuery, AvailableWebsites>(new AvailableWebsitesQuery());

        WebsiteViewModels = availableWebsites.Websites
            .Select(availableWebsite => new WebsiteViewModel(
                availableWebsite.Id,
                availableWebsite.Name,
                availableWebsite.Url))
            .ToList();
    }

    private static string BuildErrorMessage(Result result) =>
        result.Error.Code switch
        {
            Errors.General.NotFoundErrorCode => "Website not found",
            _ => "Something went wrong"
        };
}