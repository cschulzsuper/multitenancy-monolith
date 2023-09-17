using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Events
{
    [SuppressMessage("Style", "IDE1006:Naming Styles")]
    internal static class _Configure
    {
        public static EventsOptions Configure(this EventsOptions options)
        {
            options.ChannelNameResolver = provider => provider
                .GetRequiredService<ClaimsPrincipal>()
                .GetClaimOrDefault("group") ?? string.Empty;

            options.BeforeSubscriptionInvocation = BeforeSubscriptionInvocation;
            options.AfterSubscriptionInvocation = AfterSubscriptionInvocation;

            return options;
        }

        private static Task BeforeSubscriptionInvocation(IServiceProvider services, string channelName)
        {
            services
                .GetRequiredService<ClaimsPrincipal>()
                .AddIdentity(new ClaimsIdentity(new[] { new Claim("group", channelName) }));

            return Task.CompletedTask;
        }

        private static async Task AfterSubscriptionInvocation(IServiceProvider services, string _)
        {
            await services
                .GetRequiredService<IEventStorage>()
                .FlushAsync();
        }
    }
}