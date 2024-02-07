using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddGitClient(this IServiceCollection services, Action<GitClientOptions> setup)
    {
        services
            .AddOptions<GitClientOptions>()
            .Configure(setup)
            .ValidateDataAnnotations();

        services.AddSingleton<GitClient>();
        services.AddSingleton<GitRepositoryHeadInfoProvider>();

        return services;
    }
}
