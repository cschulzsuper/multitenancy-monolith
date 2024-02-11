namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class ServiceMapping
{
    public required string UniqueName { get; init; }

    public required string Url { get; init; }
}