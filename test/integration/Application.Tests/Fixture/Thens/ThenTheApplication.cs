using System.Threading;
using System.Threading.Tasks;
using Application.Test.Fixture.Shared;
using Application.UseCases.Base;
using Application.UseCases.Base.CQS;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Test.Fixture.Thens;

public class ThenTheApplication : TheApplication
{
    public ThenTheApplication(IntegrationTestWebApplicationFactory factory) : base(factory)
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
}