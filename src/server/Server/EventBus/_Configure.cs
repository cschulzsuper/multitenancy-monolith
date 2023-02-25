using System.Security.Claims;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server.EventBus;

internal static class _Configure
{
    public static EventsOptions Configure(this EventsOptions options)
    {
        options.ChannelNameResolver = provider => provider
            .GetRequiredService<ClaimsPrincipal>()
            .GetClaimOrDefault("group") ?? string.Empty;

        options.InvocationSetup = (context) => context.Services
            .GetRequiredService<ClaimsPrincipal>()
            .AddIdentity(new ClaimsIdentity(new[] {new Claim("group", context.Scope)}));

        return options;
    }
}