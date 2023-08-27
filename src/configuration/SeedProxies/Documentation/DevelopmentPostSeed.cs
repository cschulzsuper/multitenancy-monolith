using System;

namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Documentation;

public sealed class DevelopmentPostSeed
{
    public required string Title { get; init; }

    public required DateTime DateTime { get; init; }

    public required string Text { get; init; }

    public required string Link { get; init; }

    public required string[] Tags { get; init; }
}
