using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

public sealed class BadgeAuthenticationEvents
{
    public Func<BadgeValidatePrincipalContext, Task> OnValidatePrincipal { get; set; } = context => Task.CompletedTask;

    public Task ValidatePrincipal(BadgeValidatePrincipalContext context) => OnValidatePrincipal(context);
}