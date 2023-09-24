using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Extension;

public sealed class DistinctionTypeMapping : IMapping<DistinctionType>
{
    public static object SetSnowflake(DistinctionType entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(DistinctionType entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<DistinctionType> data, DistinctionType entity)
    {
        var customPropertyUniqueNameConflict = entity.CustomProperties
            .Select(x => x.UniqueName)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);

        if (customPropertyUniqueNameConflict != null)
        {
            ModelException.ThrowUniqueNameConflict<ObjectTypeCustomProperty>(customPropertyUniqueNameConflict.Key);
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<DistinctionType>(entity.UniqueName);
        }
    }
}