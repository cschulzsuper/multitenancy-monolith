namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;

public sealed class AuthenticationIdentityResponse
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }
}