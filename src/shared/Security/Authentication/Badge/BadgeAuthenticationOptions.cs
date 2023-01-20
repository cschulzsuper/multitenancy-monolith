using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Core;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

public sealed class BadgeAuthenticationOptions : AuthenticationSchemeOptions
{

    public ICollection<ClaimAction> ClaimActions { get; } = new List<ClaimAction>();

    public BadgeAuthenticationOptions()
    {
        Events = new BadgeAuthenticationEvents();
    }

    public new BadgeAuthenticationEvents Events
    {
        get => (BadgeAuthenticationEvents)base.Events!;
        set => base.Events = value;
    }
}