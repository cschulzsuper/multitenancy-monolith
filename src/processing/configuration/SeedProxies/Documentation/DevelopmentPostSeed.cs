using System;

namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies.Documentation;

public sealed class DevelopmentPostSeed
{
    public required string Project { get; init; }

    public required string Title { get; init; }

    public required DateTime Time { get; init; }

    public required string Text { get; init; }

    public required string Link { get; init; }

    public required string[] Tags { get; init; }
}
