using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Server.Middleware;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Pipeline
{
    public static IApplicationBuilder UseAuthenticationScope(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthenticationScopeMiddleware>();
    }

    public static IApplicationBuilder UseEndpointEvents(this IApplicationBuilder app)
    {
        return app.UseMiddleware<EndpointEventsMiddleware>();
    }
}