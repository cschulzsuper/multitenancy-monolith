using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Access;

public class AccountMemberModel : IModel<AccountMember>
{
    public static object SetSnowflake(AccountMember entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(AccountMember entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<AccountMember> data, AccountMember entity)
    {
        var identityUniqueNameConflict = entity.AuthenticationIdentities
            .Select(x => x.UniqueName)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);

        if (identityUniqueNameConflict != null)
        {
            ModelException.ThrowUniqueNameConflict<AccountMemberAuthenticationIdentity>(identityUniqueNameConflict.Key);
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            ModelException.ThrowUniqueNameConflict<AccountMember>(entity.UniqueName);
        }
    }
}