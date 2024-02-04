using System;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class PreBuildSerializationClientOptions
{
    [Required]
    public Type SerializationClientType { get; set; } = null!;
}