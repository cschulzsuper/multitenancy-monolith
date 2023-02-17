using ChristianSchulz.MultitenancyMonolith.Shared.Metadata;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Administration;

[ObjectAnnotation("distinction-type-custom-property",
    DisplayName = "Distinction Type Custom Property",
    Area = "administration",
    Collection = "distinction-type-custom-properties")]
public sealed class DistinctionTypeCustomProperty : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public required string UniqueName { get; set; }
}