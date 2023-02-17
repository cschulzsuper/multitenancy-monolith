namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization;

public sealed class MemberVerificationKey
{
    public required string Client { get; init; }

    public required string Identity { get; init; }

    public required string Group { get; init; }

    public required string Member { get; init; }
}