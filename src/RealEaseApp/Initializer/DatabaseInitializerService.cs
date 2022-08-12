using Application.Ports.Persistence.Write;

namespace RealEaseApp.Initializer;

public class DatabaseInitializerService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseInitializerService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = _scopeFactory.CreateScope();
        var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}