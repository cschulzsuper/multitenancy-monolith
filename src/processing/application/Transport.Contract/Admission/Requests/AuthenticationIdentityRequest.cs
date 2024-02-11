using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;

public sealed class AuthenticationIdentityRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }
}