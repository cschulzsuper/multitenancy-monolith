using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server.Json
{
    internal static class _Configure
    {
        public static IServiceCollection ConfigureJsonOptions(this IServiceCollection services)
        {
            services.Configure<JsonOptions>(options => { options.SerializerOptions.TypeInfoResolver = new CustomPropertiesResolver(); });

            return services;
        }
    }
}