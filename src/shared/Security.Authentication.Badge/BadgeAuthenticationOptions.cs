using Microsoft.AspNetCore.Authentication;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

public sealed class BadgeAuthenticationOptions : AuthenticationSchemeOptions
{
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