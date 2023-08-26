using Microsoft.AspNetCore.Components.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevDiary.Services;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IRazorComponentsBuilder AddDevDiaryServices(this IRazorComponentsBuilder builder)
    {
        return builder;
    }
}
