using Microsoft.AspNetCore.Authentication;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Authentication.Badge;

public static class _Authentication
{
    public static AuthenticationBuilder AddBadge(this AuthenticationBuilder builder, Action<BadgeAuthenticationOptions> configureOptions)
        => builder.AddScheme<BadgeAuthenticationOptions, BadgeAuthenticationHandler>("Badge", "Badge Authentication", configureOptions);
}
