﻿using ChristianSchulz.MultitenancyMonolith.Objects.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

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
            ModelException.ThrowUniqueNameConflict<Group>(entity.UniqueName);
        }
    }
}