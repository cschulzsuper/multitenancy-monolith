using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDiagnosticTransport(this IServiceCollection services)
    {
        services.AddScoped<IBuildInfoRequestHandler, BuildInfoRequestHandler>();

        return services;
    }
}