using System.Security.Claims;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.EventBus;

internal static class _Configure
{
    public static EventsOptions Configure(this EventsOptions options)
    {
        options.PublicationChannelNameResolver = provider => provider
            .GetRequiredService<ClaimsPrincipal>()
            .GetClaimOrDefault("group") ?? string.Empty;

        options.SubscriptionInvocationSetup = (services, scope) => services
            .GetRequiredService<ClaimsPrincipal>()
            .AddIdentity(new ClaimsIdentity(new[] { new Claim("group", scope) }));

        return options;
    }
}