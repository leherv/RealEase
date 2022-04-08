using System;
using System.Threading.Tasks;
using Application.Ports.Persistence.Write;
using Infrastructure.DB;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Test.Fixture.Shared;

public abstract class TheDatabase : IDisposable
{
    protected readonly TheApplication TheApplication;

    protected TheDatabase(TheApplication theApplication)
    {
        TheApplication = theApplication;
    }
    
    protected Task ExecuteDatabaseScopedAsync(Func<IUnitOfWork, Task> action)
        => TheApplication.ExecuteScopeAsync(sp =>
            action(sp.GetService<IUnitOfWork>()!)
        );
    
    protected Task<T> ExecuteDatabaseScopedAsync<T>(Func<IUnitOfWork, Task<T>> action)
        => TheApplication.ExecuteScopeAsync(sp =>
            action(sp.GetService<IUnitOfWork>()!)
        );

    protected Task ExecuteDbContextScopedAsync(Func<DatabaseContext, Task> action)
        => TheApplication.ExecuteScopeAsync(sp =>
            action(sp.GetService<DatabaseContext>()!)
        );

    protected Task<T> ExecuteDbContextScopedAsync<T>(Func<DatabaseContext, Task<T>> action)
        => TheApplication.ExecuteScopeAsync(sp =>
            action(sp.GetService<DatabaseContext>()!)
        );

    public void Dispose()
    {
        TheApplication.Dispose();
    }
}