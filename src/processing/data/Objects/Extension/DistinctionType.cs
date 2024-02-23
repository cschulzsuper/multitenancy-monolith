using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Extension;

[ObjectAnnotation(
    UniqueName = "distinction-type",
    DisplayName = "Distinction Type",
    Area = "extension",
    Collection = "distinction-types")]
public sealed class DistinctionType : ICloneable
{
    public object Clone()
    {
        var shallowCopy = new DistinctionType
        {
            Snowflake = Snowflake,
            UniqueName = UniqueName,
            DisplayName = DisplayName,
            ObjectType = ObjectType,
            CustomProperties = CustomProperties
                .Select(customProperty => (DistinctionTypeCustomProperty)customProperty.Clone())
                .ToList(),
        };

        return shallowCopy;
    }

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }

    public required string DisplayName { get; set; }

    public required string ObjectType { get; set; }

    public ICollection<DistinctionTypeCustomProperty> CustomProperties { get; init; } = [];
}