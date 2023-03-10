using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Admission;

public sealed class AuthenticationIdentityModel : IModel<AuthenticationIdentity>
{
    public static object SetSnowflake(AuthenticationIdentity entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(AuthenticationIdentity entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AuthenticationIdentity> data, AuthenticationIdentity entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<AuthenticationIdentity>(entity.UniqueName);
        }
    }
}