using System.Text.Json.Serialization;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic.Responses;

public sealed class BuildInfoResponse
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