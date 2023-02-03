﻿using ChristianSchulz.MultitenancyMonolith.Objects.Administration;
using ChristianSchulz.MultitenancyMonolith.Objects.Business;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary.Model.Business;

public class BusinessObjectModel : IModel<BusinessObject>
{
    public static object SetSnowflake(BusinessObject entity, object snowflake)
        => entity.Snowflake = (long) snowflake;

    public static object GetSnowflake(BusinessObject entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider services, IEnumerable<BusinessObject> data, BusinessObject entity)
    {
        var objectTypeRepository = services.GetRequiredService<IRepository<ObjectType>>();
        var objectType = objectTypeRepository.GetOrDefault(x => x.UniqueName == "business-object");

        foreach (var entityCustomPropertyKey in entity.CustomProperties.Keys)
        {
            var objectTypeCustomProperty = objectType?.CustomProperties.SingleOrDefault(y => y.PropertyName == entityCustomPropertyKey);
            if (objectTypeCustomProperty == null)
            {
                entity.CustomProperties.Remove(entityCustomPropertyKey);
            }
            else
            {
                var entityCustomProperty = entity.CustomProperties[entityCustomPropertyKey];
                var entityCustomPropertyType = entityCustomProperty switch
                {
                    string _ => "string",
                    _ => null
                };

                if (objectTypeCustomProperty.PropertyType != entityCustomPropertyType)
                {
                    throw new ModelException("Custom property type mismatch");
                }
            }
        }

        entity.CustomProperties = entity.CustomProperties
            .Where(x => x.Value != null!)
            .ToDictionary(x => x.Key, x => x.Value);

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            throw new ModelException("Unique name conflict");
        }
    }
}