using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Extension;

[ObjectAnnotation("object-type",
    DisplayName = "Object Type",
    Area = "extension",
    Collection = "object-types")]
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

    public ICollection<ObjectTypeCustomProperty> CustomProperties { get; init; }
        = new List<ObjectTypeCustomProperty>();
}