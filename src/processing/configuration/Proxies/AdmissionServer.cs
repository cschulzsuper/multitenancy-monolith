namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class AdmissionServer
{
    public required string Service { get; init; }
    public required string MaintenanceClient { get; init; }
    public required string MaintenanceIdentity { get; init; }
    public required string MaintenanceSecret { get; init; }
}
