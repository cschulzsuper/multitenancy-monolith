using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

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
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            throw new ModelException("Unique name conflict");
        }
    }
}