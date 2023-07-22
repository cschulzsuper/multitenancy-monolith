using ChristianSchulz.MultitenancyMonolith.Objects.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Schedule;

public sealed class JobModel : IModel<Job>
{
    public static object SetSnowflake(Job entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(Job entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider services, IEnumerable<Job> data, Job entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<Job>(entity.UniqueName);
        }
    }
}