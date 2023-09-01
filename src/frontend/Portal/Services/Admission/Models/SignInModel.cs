using System.ComponentModel.DataAnnotations;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission.Models;

public class SignInModel
{
    public string? Stage { get; set; }

    public string? ClientName { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
}
