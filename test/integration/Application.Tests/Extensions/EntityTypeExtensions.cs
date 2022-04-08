using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Application.Test.Extensions;

public static class EntityTypeExtensions
{
    public static string? GetFullTableName(this IEntityType entityType)
    {
        var schema = entityType.GetSchema();
        var tableName = entityType.GetTableName();

        return tableName == null
            ? null
            : schema != null
                ? $"{schema}.{tableName}"
                : $"\"{tableName}\"";
    }
}