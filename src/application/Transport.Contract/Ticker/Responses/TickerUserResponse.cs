namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker.Responses;

public class TickerUserResponse
{
    public required long Snowflake { get; set; }

    public required string MailAddress { get; set; }

    public required string DisplayName { get; set; }

    public required string SecretState { get; set; }
}