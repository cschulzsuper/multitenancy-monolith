using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;

public class AuthenticationIdentityRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }

    [Secret]
    public required string Secret { get; init; }
}