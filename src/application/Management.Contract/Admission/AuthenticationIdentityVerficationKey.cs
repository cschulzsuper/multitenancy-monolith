namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public sealed class AuthenticationIdentityVerificationKey
{
    public required string ClientName { get; init; }

    public required string AuthenticationIdentity { get; init; }
}