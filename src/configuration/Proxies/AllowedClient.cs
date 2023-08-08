namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class AllowedClient
{
    public required string UniqueName { get; init; }

    // TODO User WebService.Host
    public required string[] Hosts { get; init; }

    public required string[] Scopes { get; init; }
}