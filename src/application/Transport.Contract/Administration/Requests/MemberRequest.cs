using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;

public class MemberRequest
{
    [Required]
    [StringLength(140)]
    public required string UniqueName { get; init; }
}