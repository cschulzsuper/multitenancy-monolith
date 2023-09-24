using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Schedule;

public sealed class PlannedJobMapping : IMapping<PlannedJob>
{
    public static object SetSnowflake(PlannedJob entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(PlannedJob entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider services, IEnumerable<PlannedJob> data, PlannedJob entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<PlannedJob>(entity.UniqueName);
        }
    }
}