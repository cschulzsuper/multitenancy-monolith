using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Extension;

public sealed class DistinctionTypeCustomProperty : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public required string UniqueName { get; set; }
}