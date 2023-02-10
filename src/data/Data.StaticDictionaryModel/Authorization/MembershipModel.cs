﻿using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authorization;

public class MembershipModel : IModel<Membership>
{
    public static object SetSnowflake(Membership entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(Membership entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<Membership> data, Membership entity)
    {
        var membershipConflict = data.Any(x =>
            x.Identity == entity.Identity &&
            x.Group == entity.Group &&
            x.Member == entity.Member);

        if (membershipConflict)
        {
            ModelException.ThrowObjectConflict<Membership>($"{entity.Identity}/{entity.Group}/{entity.Member}");
        }
    }
}