﻿using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Access;

public sealed class AccountRegistrationModel : IModel<AccountRegistration>
{
    public static object SetSnowflake(AccountRegistration @object, object snowflake)
        => @object.Snowflake = (long)snowflake;

    public static object GetSnowflake(AccountRegistration @object)
        => @object.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AccountRegistration> data, AccountRegistration @object)
    {
        var accountGroupConflict = data.Any(x => x.AccountGroup == @object.AccountGroup);
        if (accountGroupConflict)
        {
            ModelException.ThrowPropertyValueConflict<AccountGroup>(nameof(@object.AccountGroup), @object.AccountGroup);
        }

        var processTokenConflict = data.Any(x => x.ProcessToken == @object.ProcessToken);
        if (processTokenConflict)
        {
            ModelException.ThrowPropertyValueConflict<AccountGroup>(nameof(@object.ProcessToken), @object.ProcessToken);
        }
    }
}