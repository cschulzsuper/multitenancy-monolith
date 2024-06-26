﻿using ChristianSchulz.MultitenancyMonolith.Objects.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Data.StaticDictionaryModel.Extension;

public sealed class ObjectTypeMapping : IMapping<ObjectType>
{
    public static object SetSnowflake(ObjectType entity, object snowflake)
        => entity.Snowflake == default ? entity.Snowflake = (long)snowflake : entity.Snowflake;

    public static object GetSnowflake(ObjectType entity)
        => entity.Snowflake;

    public static bool Multitenancy => true;

    public static void Ensure(IServiceProvider _, IEnumerable<ObjectType> data, ObjectType entity)
    {
        var customPropertyUniqueNameConflict = entity.CustomProperties
            .Select(x => x.UniqueName)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);

        if (customPropertyUniqueNameConflict != null)
        {
            DataException.ThrowUniqueNameConflict<ObjectTypeCustomProperty>(customPropertyUniqueNameConflict.Key);
        }

        var customPropertyNameConflict = entity.CustomProperties
            .Select(x => x.PropertyName)
            .GroupBy(x => x)
            .FirstOrDefault(x => x.Count() > 1);

        if (customPropertyNameConflict != null)
        {
            DataException.ThrowPropertyNameConflict<ObjectTypeCustomProperty>(customPropertyNameConflict.Key);
        }

        var uniqueNameConflict = data.Any(x => x.UniqueName == entity.UniqueName);
        if (uniqueNameConflict)
        {
            DataException.ThrowUniqueNameConflict<ObjectTypeCustomProperty>(entity.UniqueName);
        }
    }
}