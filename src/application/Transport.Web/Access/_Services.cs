using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Services
{
    public static IServiceCollection AddAccessWebServiceTransportClients(this IServiceCollection services)
    {
        services.AddWebServiceTransportDefaultClient<IContextAccountMemberCommandClient>(
            configuration => configuration.GetAccessServer().Service);

        return services;
    }
}