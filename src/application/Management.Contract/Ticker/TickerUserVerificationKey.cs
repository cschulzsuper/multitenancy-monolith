namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public sealed class TickerUserVerificationKey
{
    public required string ClientName { get; init; }

    public required string AccountGroup { get; init; }

    public required string Mail { get; init; }
}