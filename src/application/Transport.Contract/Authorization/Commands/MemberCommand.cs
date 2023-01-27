using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authorization.Commands;

public class MemberCommand
{
    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}