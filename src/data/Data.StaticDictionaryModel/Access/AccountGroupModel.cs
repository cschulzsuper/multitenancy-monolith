using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Access;

public sealed class AccountGroupModel : IModel<AccountGroup>
{
    public static object SetSnowflake(AccountGroup entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(AccountGroup entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AccountGroup> data, AccountGroup entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<AccountGroup>(entity.UniqueName);
        }
    }
}