using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Test.Fixture.Shared;

public abstract class TheApplication : IDisposable
{
    protected readonly IntegrationTestWebApplicationFactory Factory;
    
    protected TheApplication(IntegrationTestWebApplicationFactory factory)
    {
        Factory = factory;
    }
    
    private IServiceScopeFactory GetServiceScopeFactory()
    {
        return Factory.Services.GetService<IServiceScopeFactory>()!;
    }
    
    public async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
    {
        using var scope = GetServiceScopeFactory().CreateScope();
        var result = await action(scope.ServiceProvider);
        return result;
    }

    public async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
    {
        using var scope = GetServiceScopeFactory().CreateScope();
        await action(scope.ServiceProvider);
    }
    
    public void Dispose()
    {
        Factory.Dispose();
    }
}