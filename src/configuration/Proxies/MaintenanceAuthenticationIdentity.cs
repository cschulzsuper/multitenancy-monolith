namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class MaintenanceAuthenticationIdentity
{
    public required string ClientName { get; init; }
    public required string UniqueName { get; init; }
    public required string Secret { get; init; }
}
