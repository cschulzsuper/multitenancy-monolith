using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authentication;

public class IdentityModel : IModel<Identity>
{
    public static object SetSnowflake(Identity entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(Identity entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<Identity> data, Identity entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<Identity>(entity.UniqueName);
        }
    }
}