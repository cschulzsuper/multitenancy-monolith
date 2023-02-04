using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Authorization;

public class GroupModel : IModel<Group>
{
    public static object SetSnowflake(Group entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(Group entity)
        => entity.Snowflake;

    public static bool Multitenancy => false;

    public static void Ensure(IServiceProvider _, IEnumerable<Group> data, Group entity)
    {
        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            throw new ModelException("Unique name conflict");
        }
    }
}