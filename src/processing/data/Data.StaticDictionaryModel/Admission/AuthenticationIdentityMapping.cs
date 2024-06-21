using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Admission;

public sealed class AuthenticationIdentityMapping : IMapping<AuthenticationIdentity>
{
    public static object SetSnowflake(AuthenticationIdentity entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(AuthenticationIdentity entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AuthenticationIdentity> data, AuthenticationIdentity entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            DataException.ThrowUniqueNameConflict<AuthenticationIdentity>(entity.UniqueName);
        }
    }
}