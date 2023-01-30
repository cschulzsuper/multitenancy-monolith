namespace ChristianSchulz.MultitenancyMonolith.Objects.Authentication;

public sealed class Identity : ICloneable
{
    public object Clone()
        => MemberwiseClone();

    public long Snowflake { get; set; }

    public required string UniqueName { get; set; }
    public required string MailAddress { get; set; }
    public required string Secret { get; set; }
}