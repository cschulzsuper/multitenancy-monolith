using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authorization;

public class MemberModel : IModel<Member>
{
    public static object SetSnowflake(Member entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(Member entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<Member> data, Member entity)
    {
        var identityUniqueNameConflict = entity.Identities
            .Select(x => x.UniqueName)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);

        if (identityUniqueNameConflict != null)
        {
            ModelException.ThrowUniqueNameConflict<MemberIdentity>(identityUniqueNameConflict.Key);
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<Member>(entity.UniqueName);
        }
    }
}