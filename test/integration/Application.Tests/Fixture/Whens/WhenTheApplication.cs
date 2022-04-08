using System.Threading;
using System.Threading.Tasks;
using Application.Test.Fixture.Shared;
using Application.UseCases.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Test.Fixture.Whens;

public class WhenTheApplication : TheApplication
{
    public WhenTheApplication(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }
    
    public Task<TQueryResult> ReceivesQuery<TQuery, TQueryResult>(TQuery query)
    {
        return ExecuteScopeAsync(sp =>
        {
            var queryDispatcher = sp.GetRequiredService<IQueryDispatcher>();
    
            return queryDispatcher.Dispatch<TQuery, TQueryResult>(query, CancellationToken.None);
        });
    }
    
    public Task<TCommandResult> ReceivesCommand<TCommand, TCommandResult>(TCommand command)
    {
        return ExecuteScopeAsync(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
    
            return commandDispatcher.Dispatch<TCommand, TCommandResult>(command, CancellationToken.None);
        });
    }
}