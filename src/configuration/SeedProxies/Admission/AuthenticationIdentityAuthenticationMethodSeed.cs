namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;

public sealed class AuthenticationIdentityAuthenticationMethodSeed
{
    public required string ClientName { get; init; }

    public required string AuthenticationMethod { get; init; }

    public required string AuthenticationIdentity { get; init; }
}
