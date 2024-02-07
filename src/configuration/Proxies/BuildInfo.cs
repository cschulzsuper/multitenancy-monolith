namespace ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;

public sealed class BuildInfo
{
    public string? BranchName { get; init; }

    public string? CommitHash { get; init; }

    public string? ShortCommitHash { get; init; }

    public string? BuildNumber { get; init; }

}
