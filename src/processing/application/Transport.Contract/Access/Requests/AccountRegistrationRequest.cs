using ChristianSchulz.MultitenancyMonolith.Shared.Validation.PredefinedAnnotations;
using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access.Requests;

public sealed class AccountRegistrationRequest
{
    [Display(Name = "authentication identity")]
    [UniqueName]
    public required string AuthenticationIdentity { get; set; }

    [Display(Name = "account group")]
    [UniqueName]
    public required string AccountGroup { get; set; }

    [Display(Name = "account member")]
    [UniqueName]
    public required string AccountMember { get; set; }

    [MailAddress]
    public required string MailAddress { get; set; }
}