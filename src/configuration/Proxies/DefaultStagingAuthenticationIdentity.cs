namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class DefaultStagingAuthenticationIdentity
{
    public required string UniqueName { get; init; }
    public required string Secret { get; init; }
}
