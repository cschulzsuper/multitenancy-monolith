using ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IRazorComponentsBuilder AddPortalServices(this IRazorComponentsBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<AuthService>();

        return builder;
    }
}
