﻿using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Access;

public sealed class AccountMemberMapping : IMapping<AccountMember>
{
    public static object SetSnowflake(AccountMember entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

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
            DataException.ThrowUniqueNameConflict<AccountMemberAuthenticationIdentity>(identityUniqueNameConflict.Key);
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            DataException.ThrowUniqueNameConflict<AccountMember>(entity.UniqueName);
        }
    }
}