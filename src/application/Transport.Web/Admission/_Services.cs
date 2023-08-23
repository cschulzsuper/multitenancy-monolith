using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAdmissionTransportWebServiceClients(this IServiceCollection services)
    {
        services.AddWebServiceTransportDefaultClient<IContextAuthenticationIdentityCommandClient>(
            configuration => configuration.GetAdmissionServer().Service);

        return services;
    }
}