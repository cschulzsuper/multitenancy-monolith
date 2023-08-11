namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class AllowedClient
{
    public required string Service { get; init; }

    public required string[] Scopes { get; init; }
}