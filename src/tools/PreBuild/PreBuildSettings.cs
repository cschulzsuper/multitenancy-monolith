using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild;

public sealed class PreBuildSettings
{
    public string? SourceDirectoryPath { get; init; }

    public string? OutputDirectoryPath { get; init; }

    public string? OutputFilenameFormat { get; init; }

    [AllowedValues("json", "values")]
    public string OutputFormat { get; init; } = "json";

    public string? BranchName { get; init; }

    public string? CommitHash { get; init; }

    public string? ShortCommitHash { get; init; }

    public string? BuildNumber { get; init; }

}
