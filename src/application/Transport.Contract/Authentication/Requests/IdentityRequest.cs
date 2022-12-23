namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication.Requests;

public class IdentityRequest
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }

    public required string Secret { get; init; }
}