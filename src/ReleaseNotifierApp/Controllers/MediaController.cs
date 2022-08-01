using Application.UseCases.Base;
using Application.UseCases.Media.QueryAvailableMedia;
using Microsoft.AspNetCore.Mvc;

namespace ReleaseNotifierApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController
{
    private readonly IQueryDispatcher _queryDispatcher;

    public MediaController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<AvailableMedia> GetAvailable()
    {
        var availableMedia =
            await _queryDispatcher.Dispatch<AvailableMediaQuery, AvailableMedia>(new AvailableMediaQuery());
        
        return availableMedia;
    }
}