using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DB.Conventions;

public interface IConvention
{
    /// <summary>
    ///     Applies a custom convention to the <see cref="IMutableModel" />.
    /// </summary>
    void Apply(IMutableModel model);
}