namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public sealed class IdentityVerificationKey
{
    public required string Client { get; init; }

    public required string Identity { get; init; }
}