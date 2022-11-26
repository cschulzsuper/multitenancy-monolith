using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

public static class _Services
{
    public static IServiceCollection AddRequestUser(this IServiceCollection services)
        => services
            .AddScoped<ClaimsPrincipalContext>()
            .AddScoped(provider => provider.GetRequiredService<ClaimsPrincipalContext>().User)
            .AddScoped<IClaimsTransformation, ClaimsPrincipalTransformation>();
}
