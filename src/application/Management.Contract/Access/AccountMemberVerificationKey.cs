namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public sealed class AccountMemberVerificationKey
{
    public required string ClientName { get; init; }

    public required string AuthenticationIdentity { get; init; }

    public required string AccountGroup { get; init; }

    public required string AccountMember { get; init; }
}