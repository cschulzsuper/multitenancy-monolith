using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Admission.ConcreteAnnotations;
using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;

public sealed class ContextAuthenticationIdentityAuthCommand
{
    [Display(Name = "authentication identity")]
    [UniqueName]
    public required string AuthenticationIdentity { get; init; }

    [Display(Name = "authentication method type")]
    [AuthenticationMethod]
    public string? AuthenticationMethod { get; init; }

    [Secret]
    public string? Secret { get; init; }

    [Display(Name = "client name")]
    [Required]
    [StringLength(140)]
    public required string ClientName { get; init; }
}