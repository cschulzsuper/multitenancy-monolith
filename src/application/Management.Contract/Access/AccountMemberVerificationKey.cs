namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public sealed class AccountMemberVerificationKey
{
    public required string Client { get; init; }

    public required string Identity { get; init; }

    public required string Group { get; init; }

    public required string Member { get; init; }
}