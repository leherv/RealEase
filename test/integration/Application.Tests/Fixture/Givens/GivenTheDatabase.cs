using System.Threading.Tasks;
using Application.Test.Extensions;
using Application.Test.Fixture.Shared;

namespace Application.Test.Fixture.Givens;

public class GivenTheDatabase : TheDatabase
{
    private readonly GivenTheData _givenTheData;

    public GivenTheDatabase(
        GivenTheData givenTheData,
        TheApplication theApplication
    ) : base(theApplication)
    {
        _givenTheData = givenTheData;
    }

    public async Task IsCleared()
    {
        await ExecuteDbContextScopedAsync(async databaseContext =>
        {
            databaseContext.Clear();
            await databaseContext.SaveChangesAsync();
        });
    }

    public async Task IsSeeded()
    {
        await ExecuteDbContextScopedAsync(async databaseContext =>
        {
            databaseContext.Seed(_givenTheData);
            await databaseContext.SaveChangesAsync();
        });
    }
}