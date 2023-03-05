namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;

public class AuthenticationIdentityResponse
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }
}