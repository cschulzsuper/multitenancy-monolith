namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class WebService
{
    public required string UniqueName { get; init; }

    public required string Host { get; init; }
}