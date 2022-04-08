using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DB.Conventions;

/// <summary>
///     Configures EF Core to use the entity name as the table name. Otherwise the name of the
///     <see cref="DbSet{TEntity}" /> property would be used.
/// </summary>
public class EntityNamingConvention : IConvention
{
    public void Apply(IMutableModel model)
    {
        foreach (var entity in model.GetEntityTypes().Where(entity => !entity.IsOwned() && entity.BaseType == null))
            entity.SetTableName(entity.DisplayName());
    }
}