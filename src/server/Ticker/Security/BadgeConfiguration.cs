using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using Microsoft.Extensions.DependencyInjection;


namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;

internal static class BadgeConfiguration
{
    public static void Configure(this BadgeAuthenticationOptions options)
    {
        options.Events.OnValidatePrincipal = async context =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<BadgeValidator>() ?? new BadgeValidator();

            var valid = await validator.ValidateAsync(context);

            if (!valid)
            {
                context.RejectPrincipal();
            }
        };
    }
}