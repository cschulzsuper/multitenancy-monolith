namespace ChristianSchulz.MultitenancyMonolith.Objects.Administration;

public sealed class DistinctionTypeCustomProperty : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public required string UniqueName { get; set; }
}