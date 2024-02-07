using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;

public sealed class GitClientOptions
{
    [Required]
    public string RepositoryPath { get; set; } = null!;
}