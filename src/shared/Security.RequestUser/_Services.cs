using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddRequestUser(this IServiceCollection services)
        => services
            .AddScoped<ClaimsPrincipalContext>()
            .AddTransient(provider => provider.GetRequiredService<ClaimsPrincipalContext>().User)
            .AddScoped<IClaimsTransformation, ClaimsPrincipalTransformation>();
}