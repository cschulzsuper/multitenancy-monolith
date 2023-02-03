namespace ChristianSchulz.MultitenancyMonolith.Objects.Business;

public sealed class BusinessObject : ICloneable
{
    public object Clone()
    {
        var shallowCopy = new BusinessObject
        {
            Snowflake = Snowflake,
            UniqueName = UniqueName,
            CustomProperties = CustomProperties
                .ToDictionary(
                    x => x.Key,
                    x => x.Value)
        };

        return shallowCopy;
    }

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }


    public IDictionary<string, object> CustomProperties { get; set; } = new Dictionary<string, object>();

    public object this[string propertyName]
    {
        get => CustomProperties[propertyName];
        set => CustomProperties[propertyName] = value;
    }
}