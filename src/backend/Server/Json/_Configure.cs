using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.Json;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
internal static class _Configure
{
    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options => { options.SerializerOptions.TypeInfoResolverChain.Insert(0,new CustomPropertiesResolver()); });

        return services;
    }
}