using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission.Requests;

public sealed class AuthenticationRegistrationRequest
{
    [Display(Name = "authentication identity")]
    [UniqueName]
    public required string AuthenticationIdentity { get; set; }

    [MailAddress]
    public required string MailAddress { get; set; }
}