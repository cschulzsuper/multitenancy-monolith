using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge;

public class BadgeAuthenticationEvents
{
    public Func<BadgeValidatePrincipalContext, Task> OnValidatePrincipal { get; set; } = context => Task.CompletedTask;

    public virtual Task ValidatePrincipal(BadgeValidatePrincipalContext context) => OnValidatePrincipal(context);
}
