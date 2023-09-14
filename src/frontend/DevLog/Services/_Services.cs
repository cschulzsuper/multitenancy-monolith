using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services.Documentation;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IRazorComponentsBuilder AddDevLogServices(this IRazorComponentsBuilder builder)
    {

        builder.Services.AddScoped<DevelopmentPostService>();

        return builder;
    }
}
