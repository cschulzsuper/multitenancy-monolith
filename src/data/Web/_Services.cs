using System;
using System.Diagnostics;
using System.Linq;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Web;

public static class _Services
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, params string[] uniqueNames)
    {
        foreach (var uniqueName in uniqueNames)
        {
            services.AddHttpClient(uniqueName,
                (services, httpClient) =>
                {
                    var webServices = services
                        .GetRequiredService<IWebServicesProvider>()
                        .Get();

                    var baseAddress = webServices.SingleOrDefault(x => x.UniqueName == uniqueName);

                    if(baseAddress == null)
                    {
                        throw new UnreachableException($"Client {uniqueName} is not configured");
                    }

                    httpClient.BaseAddress = new Uri(baseAddress.Host);
                    httpClient.Timeout = TimeSpan.FromMilliseconds(250);
                });
        }

        services.AddScoped<IWebServiceClientFactory, WebServiceClientFactory>();

        return services;
    }
}
