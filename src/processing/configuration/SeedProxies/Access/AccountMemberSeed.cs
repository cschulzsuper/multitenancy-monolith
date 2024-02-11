namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Access;

public sealed class AccountMemberSeed
{
    public required string AccountGroup { get; init; }

    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }

    public required string Secret { get; init; }

    public required string[] AuthenticationIdentities { get; init; }
}
