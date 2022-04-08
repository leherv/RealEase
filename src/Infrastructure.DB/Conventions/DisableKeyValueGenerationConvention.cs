using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DB.Conventions;

/// <summary>
///     Configures EF Core to disable key value generation for all entities.
/// </summary>
public class DisableKeyValueGenerationConvention : IConvention
{
    public void Apply(IMutableModel model)
    {
        foreach (var property in model
                     .GetEntityTypes()
                     .Where(entity => entity.BaseType == null && !entity.IsKeyless)
                     .Select(entity => entity.FindProperty("Id"))
                     .Where(entity => entity != null)
                     .Select(entity => entity!))
        {
            property.ValueGenerated = ValueGenerated.Never;
        }
    }
}