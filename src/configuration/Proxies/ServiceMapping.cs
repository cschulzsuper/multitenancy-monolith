namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class ServiceMapping
{
    public required string UniqueName { get; init; }

    public required string PublicUrl { get; init; }

    public required string ServiceUrl { get; init; }
}