namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services.Diagnostic.Models;

public class BuildInfoModel
{
    public required string? BranchName { get; init; }

    public required string? CommitHash { get; init; }

    public required string? ShortCommitHash { get; init; }

    public required string? BuildNumber { get; init; }

}
