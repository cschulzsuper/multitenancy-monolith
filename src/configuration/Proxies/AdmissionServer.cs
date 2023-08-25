namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class AdmissionServer
{
    public required string ClientName { get; init; }

    public string? FrontendService { get; init; }
    public required string BackendService { get; init; }
    
    public string? MaintenanceAuthenticationIdentityClientName { get; init; }
    public string? MaintenanceAuthenticationIdentityUniqueName { get; init; }
    public string? MaintenanceAuthenticationIdentitySecret { get; init; }
}
