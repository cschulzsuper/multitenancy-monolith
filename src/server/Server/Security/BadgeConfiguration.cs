using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security;

internal static class BadgeConfiguration
{
    public static void Configure(this BadgeAuthenticationOptions options)
    {
        options.Events.OnValidatePrincipal = context =>
        {
            var validator = context.HttpContext.RequestServices
                .GetService<BadgeValidator>() ?? new BadgeValidator();

            var valid = validator.Validate(context);
            if (!valid)
            {
                context.RejectPrincipal();
            }

            return Task.CompletedTask;
        };
    }
}