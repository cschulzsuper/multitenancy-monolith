namespace ChristianSchulz.MultitenancyMonolith.Objects.Administration;

public sealed class ObjectTypeCustomProperty : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public required string UniqueName { get; set; }

    public required string DisplayName { get; set; }

    public required string PropertyName { get; set; }

    public required string PropertyType { get; set; }
}