using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _ServiceScopes
{
    public static IServiceScope CreateMultitenancyScope(this IServiceProvider services, string multitenancyDiscriminator)
    {
        var scope = services.CreateScope();

        scope.ServiceProvider
            .GetRequiredService<MultitenancyContext>()
            .MultitenancyDiscriminator = multitenancyDiscriminator;

        return scope;
    }
}
