using Application.Ports.General;
using Microsoft.Extensions.Logging;

namespace Infrastructure.General.Adapters;

public class ApplicationLogger : IApplicationLogger
{
    private readonly ILogger<ApplicationLogger> _logger;

    public ApplicationLogger(ILogger<ApplicationLogger> logger)
    {
        _logger = logger;
    }
}