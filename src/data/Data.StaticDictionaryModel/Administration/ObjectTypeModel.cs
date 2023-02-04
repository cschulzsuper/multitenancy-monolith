using ChristianSchulz.MultitenancyMonolith.Objects.Administration;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Administration;

public class ObjectTypeModel : IModel<ObjectType>
{
    public static object SetSnowflake(ObjectType entity, object snowflake)
        => entity.Snowflake = (long)snowflake;

    public static object GetSnowflake(ObjectType entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<ObjectType> data, ObjectType entity)
    {
        var customPropertyUniqueNameConflict = entity.CustomProperties
            .Select(x => x.UniqueName)
            .Distinct()
            .Count() != entity.CustomProperties.Count;

        if (customPropertyUniqueNameConflict)
        {
            throw new ModelException("Custom property unqiue name conflict");
        }

        var customPropertyNameConflict = entity.CustomProperties
            .Select(x => x.PropertyName)
            .Distinct()
            .Count() != entity.CustomProperties.Count;

        if (customPropertyNameConflict)
        {
            throw new ModelException("Custom property name conflict");
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            throw new ModelException("Unique name conflict");
        }
    }
}