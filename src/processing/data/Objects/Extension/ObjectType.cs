using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Extension;

public sealed class ObjectType : ICloneable
{
    public object Clone()
    {
        var shallowCopy = new ObjectType
        {
            Snowflake = Snowflake,
            UniqueName = UniqueName,
            CustomProperties = CustomProperties
                .Select(customProperty => (ObjectTypeCustomProperty)customProperty.Clone())
                .ToList(),
        };

        return shallowCopy;
    }

    public long Snowflake { get; set; }

    public required string UniqueName { get; init; }

    public ICollection<ObjectTypeCustomProperty> CustomProperties { get; init; } = [];
}