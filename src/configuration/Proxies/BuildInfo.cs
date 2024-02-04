namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class BuildInfo
{
    public string? Branch { get; init; }

    public string? CommitHash { get; init; }

    public string? ShortCommitHash { get; init; }

    public string? Build { get; init; }

}
