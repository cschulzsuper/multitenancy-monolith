using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration.Requests;

public class MemberSignInRequest
{
    [Required]
    [StringLength(140)]
    public required string Client { get; init; }
}