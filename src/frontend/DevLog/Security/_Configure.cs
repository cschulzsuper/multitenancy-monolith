using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Security;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Configure
{
    public static RequestUserOptions Configure(this RequestUserOptions options, ICollection<AllowedClient> allowedClients)
    {
        options.Transform = user => RequestUserConfiguration.TransformAsync(user, allowedClients);

        return options;
    }

    public static BearerTokenOptions Configure(this BearerTokenOptions options)
    {
        options.Events.OnMessageReceived = BearerTokenMessageHandler.Handle<BearerTokenValidator>;

        return options;
    }


}