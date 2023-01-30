using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;

public class IdentityRequest
{
    [UniqueName]
    public required string UniqueName { get; init; }

    [MailAddress]
    public required string MailAddress { get; init; }

    [Secret]
    public required string Secret { get; init; }
}