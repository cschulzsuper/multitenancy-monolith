using System;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class PreBuildOutputOptions
{
    [Required]
    public Type OutputType { get; set; } = null!;
}