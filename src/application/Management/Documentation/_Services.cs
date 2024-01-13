using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Documentation;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddDocumentationManagement(this IServiceCollection services)
    {
        services.AddScoped<IDevelopmentPostManager, DevelopmentPostManager>();

        return services;
    }
}