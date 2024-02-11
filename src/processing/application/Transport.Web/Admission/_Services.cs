using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddAdmissionTransportWebServiceClients(this IServiceCollection services)
    {
        services.AddWebServiceTransportDefaultClient<IAuthenticationIdentityRequestClient>(
            configuration => configuration.GetAdmissionServer().Service);

        services.AddWebServiceTransportDefaultClient<IContextAuthenticationIdentityCommandClient>(
            configuration => configuration.GetAdmissionServer().Service);

        return services;
    }
}