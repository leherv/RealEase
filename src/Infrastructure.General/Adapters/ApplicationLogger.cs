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

    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    public void LogWarning(Exception e, string message)
    {
        _logger.LogWarning(e, message);
    }
}