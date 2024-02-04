using System.Text.Json.Serialization;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild;

public sealed class BuildInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? BranchName { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? CommitHash { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? ShortCommitHash { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? BuildNumber { get; init; }
}