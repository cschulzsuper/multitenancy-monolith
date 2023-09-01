using System;

namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Ticker;

public sealed class TickerUserSeed
{
    public required string AccountGroup { get; init; }

    public required string DisplayName { get; init; }

    public required string MailAddress { get; init; }

    public required string Secret { get; init; }

    public required string SecretState { get; init; }

    public required Guid SecretToken { get; init; }
}
