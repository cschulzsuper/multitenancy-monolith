using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server
{
    public static class _Pipeline
    {
        public static IApplicationBuilder UseRequestUser(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var claimsPrincipalContext = context.RequestServices.GetRequiredService<ClaimsPrincipalContext>();

                claimsPrincipalContext.User = context.User;

                await next.Invoke(context);
            });

            return app;
        }
    }
}
