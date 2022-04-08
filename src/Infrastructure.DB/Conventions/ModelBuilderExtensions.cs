using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DB.Conventions;

public static class ModelBuilderExtensions
{
    public static ModelBuilder UseConvention<TConvention>(this ModelBuilder modelBuilder)
        where TConvention : IConvention, new()
    {
        return modelBuilder.UseConvention(new TConvention());
    }

    public static ModelBuilder UseConvention(this ModelBuilder modelBuilder, IConvention convention)
    {
        convention.Apply(modelBuilder.Model);

        return modelBuilder;
    }
}