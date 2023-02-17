using Microsoft.AspNetCore.Authentication;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Authentication
{
    public static AuthenticationBuilder AddBadge(this AuthenticationBuilder builder, Action<BadgeAuthenticationOptions> configureOptions)
        => builder.AddScheme<BadgeAuthenticationOptions, BadgeAuthenticationHandler>("Badge", "Badge Authentication", configureOptions);
}