namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;

public sealed class AccountMemberResponse
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }
}