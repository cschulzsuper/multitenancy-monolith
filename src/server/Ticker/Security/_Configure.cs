using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using Microsoft.AspNetCore.Authentication.BearerToken;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

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
        //options.BearerTokenProtector = new BearerTokenProtector();
        
        options.Events.OnMessageReceived = async context =>
        {
            context.Token =
                BearerTokenSource.GetTokenFromHeaders(context.HttpContext) ??
                BearerTokenSource.GetTokenFromCookies(context.HttpContext) ??
                BearerTokenSource.GetTokenFromQuery(context.HttpContext);

            var ticket = context.Options.BearerTokenProtector.Unprotect(context.Token);
            if (ticket == null)
            {
                context.Fail("Unprotected token failed");
                return;
            }

            await new BearerTokenValidator().ValidateAsync(context, ticket);
        };

        return options;
    }
}