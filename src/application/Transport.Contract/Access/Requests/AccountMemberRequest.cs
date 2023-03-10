using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;

public sealed class AccountMemberRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }
}