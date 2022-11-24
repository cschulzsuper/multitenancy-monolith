using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server
{
    public static class _Services
    {
        public static IServiceCollection AddRequestUser(this IServiceCollection services)
            => services
                .AddScoped<ClaimsPrincipalContext>()
                .AddScoped(provider => provider.GetRequiredService<ClaimsPrincipalContext>().User);
    }
}
