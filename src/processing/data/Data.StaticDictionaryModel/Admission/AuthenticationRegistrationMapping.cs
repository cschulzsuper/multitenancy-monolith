﻿using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Admission;

public sealed class AuthenticationRegistrationMapping : IMapping<AuthenticationRegistration>
{
    public static object SetSnowflake(AuthenticationRegistration @object, object snowflake)
        => @object.Snowflake = (long)snowflake;

    public static object GetSnowflake(AuthenticationRegistration @object)
        => @object.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AuthenticationRegistration> data, AuthenticationRegistration @object)
    {
        var authenticationIdentityConflict = data.Any(x => x.AuthenticationIdentity == @object.AuthenticationIdentity);
        if (authenticationIdentityConflict)
        {
            DataException.ThrowPropertyValueConflict<AccountGroup>(nameof(@object.AuthenticationIdentity), @object.AuthenticationIdentity);
        }
    }
}