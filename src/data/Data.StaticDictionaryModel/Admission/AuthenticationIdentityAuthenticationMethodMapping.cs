﻿using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Admission;

public sealed class AuthenticationIdentityAuthenticationMethodMapping : IMapping<AuthenticationIdentityAuthenticationMethod>
{
    public static object SetSnowflake(AuthenticationIdentityAuthenticationMethod entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(AuthenticationIdentityAuthenticationMethod entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<AuthenticationIdentityAuthenticationMethod> data, AuthenticationIdentityAuthenticationMethod entity)
    {
        var entityConflict = data.Any(x => 
            x.ClientName == entity.ClientName && 
            x.AuthenticationIdentity == entity.AuthenticationIdentity && 
            x.AuthenticationMethod == entity.AuthenticationMethod);

        if (entityConflict)
        {
            ModelException.ThrowObjectConflict<AuthenticationIdentity>($"{entity.AuthenticationIdentity}/{entity.ClientName}/{entity.AuthenticationMethod}");
        }
    }
}