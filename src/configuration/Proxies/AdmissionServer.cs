namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class AdmissionServer
{
    public required string Service { get; init; }
    public string? MaintenanceAuthenticationIdentity { get; init; }
    public string? MaintenanceAuthenticationIdentitySecret { get; init; }
}
