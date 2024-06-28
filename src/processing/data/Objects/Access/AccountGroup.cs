using System;

namespace ChristianSchulz.MultitenancyMonolith.Objects.Access;

public sealed class AccountGroup : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
}