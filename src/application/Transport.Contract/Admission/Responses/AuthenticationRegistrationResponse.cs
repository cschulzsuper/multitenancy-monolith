namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Responses;

public sealed class AuthenticationRegistrationResponse
{
    public required long Snowflake { get; init; }

    public required string AuthenticationIdentity { get; init; }

    public required string MailAddress { get; init; }

    public required object ProcessState { get; init; }
}