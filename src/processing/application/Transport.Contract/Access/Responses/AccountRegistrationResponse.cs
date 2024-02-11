namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Responses;

public sealed class AccountRegistrationResponse
{
    public required long Snowflake { get; set; }

    public required string AuthenticationIdentity { get; set; }

    public required string AccountGroup { get; set; }

    public required string AccountMember { get; set; }

    public required string MailAddress { get; set; }

    public required string ProcessState { get; set; }
}