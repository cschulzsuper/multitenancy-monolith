namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Responses;

public class IdentityResponse
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }
}