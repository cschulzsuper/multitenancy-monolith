using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Outputs;

public sealed class FileOutputOptions
{
    [Required]
    public string OutputDirectoryPath { get; set; } = null!;

    [Required]
    public string OutputFilenameFormat { get; set; } = null!;
}