using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Authorization;

public class MembershipModel : IModel<Membership>
{
    public static object SetSnowflake(Membership entity, object snowflake)
        => entity.Snowflake = (long) snowflake;

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
            throw new ModelException("Membership conflict");
        }
    }
}