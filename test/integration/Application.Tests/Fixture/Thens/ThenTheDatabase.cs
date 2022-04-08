using System;
using System.Threading.Tasks;
using Application.Ports.Persistence.Write;
using Application.Test.Fixture.Shared;
using Domain.Model;
using Infrastructure.DB;

namespace Application.Test.Fixture.Thens;

public class ThenTheDatabase : TheDatabase
{
    public ThenTheDatabase(TheApplication theApplication) : base(theApplication)
    {
    }

    // Should be used when no useCase can be used to verify state
    public Task<T> Query<T>(Func<IUnitOfWork, Task<T>> action)
        => ExecuteDatabaseScopedAsync(action);

    // Should be used when 1. no useCase and 2. IUnitOfWork can not be used to verify state (as close to client as possible)
    public Task<T> Query<T>(Func<DatabaseContext, Task<T>> action)
        => ExecuteDbContextScopedAsync(action);

    public async Task<Subscriber?> GetSubscriberForExternalIdentifier(string externalIdentifier)
    {
        return await Query(unitOfWork => unitOfWork.SubscriberRepository.GetByExternalId(externalIdentifier));
    }

   
}