namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public sealed class IdentityVerificationKey
{
    public required string Client { get; init; }

    public required string Identity { get; init; }
}