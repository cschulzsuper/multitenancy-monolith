using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Documentation;

public sealed class DevelopmentPostMapping : IMapping<DevelopmentPost>
{
    public static object SetSnowflake(DevelopmentPost entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(DevelopmentPost entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<DevelopmentPost> data, DevelopmentPost entity)
    {
        var indexConflict = data.Any(x => x.Index == entity.Index);
        if (indexConflict)
        {
            DataException.ThrowObjectConflict<DevelopmentPost>($"{entity.Index}");
        }
    }
}