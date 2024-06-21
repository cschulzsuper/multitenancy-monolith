using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.Json;

internal static class _Configure
{
    public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options => { options.SerializerOptions.TypeInfoResolverChain.Insert(0,new CustomPropertiesResolver()); });
        //services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<JsonOptions>, ProblemDetailsJsonOptionsSetup1>());

        return services;
    }
}