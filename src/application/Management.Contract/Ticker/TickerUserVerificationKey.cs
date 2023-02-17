namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

public sealed class TickerUserVerificationKey
{
    public required string Client { get; init; }

    public required string Group { get; init; }

    public required string Mail { get; init; }
}