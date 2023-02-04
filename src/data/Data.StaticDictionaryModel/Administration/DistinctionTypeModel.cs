using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel;
using ChristianSchulz.MultitenancyMonolith.Objects.Administration;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Administration;

public class DistinctionTypeModel : IModel<DistinctionType>
{
    public static object SetSnowflake(DistinctionType entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(DistinctionType entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<DistinctionType> data, DistinctionType entity)
    {
        var customPropertyConflict = entity.CustomProperties
            .Select(x => x.UniqueName)
            .Distinct()
            .Count() != entity.CustomProperties.Count;

        if (customPropertyConflict)
        {
            throw new ModelException("Custom property unique name conflict");
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            throw new ModelException("Unique name conflict");
        }
    }
}