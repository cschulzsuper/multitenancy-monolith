namespace ChristianSchulz.MultitenancyMonolith.Objects.Administration;

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
                .Select(customProperty => (DistinctionTypeCustomProperty) customProperty.Clone())
                .ToList(),
        };

        return shallowCopy;
    }

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }

    public required string DisplayName { get; set; }

    public required string ObjectType { get; set; }

    public ICollection<DistinctionTypeCustomProperty> CustomProperties { get; init; }
        = new List<DistinctionTypeCustomProperty>();
}