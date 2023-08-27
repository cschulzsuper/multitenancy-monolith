using System;

namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Admission;

public sealed class AuthenticationIdentitySeed
{
    public required string UniqueName { get; init; }

    public required string MailAddress { get; init; }

    public required string Secret { get; init; }
}
