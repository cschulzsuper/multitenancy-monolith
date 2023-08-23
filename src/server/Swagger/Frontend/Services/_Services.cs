using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Frontend.Services;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IRazorComponentsBuilder AddFrontendServices(this IRazorComponentsBuilder builder)
    {
        builder.Services.AddScoped<SignInService>();

        return builder;
    }
}
