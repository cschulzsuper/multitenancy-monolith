using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Outputs;

public sealed class GitClientOptions
{
    [Required]
    public string RepositoryPath { get; set; } = null!;
}