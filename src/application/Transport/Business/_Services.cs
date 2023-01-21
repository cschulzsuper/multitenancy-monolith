using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public static class _Services
{
    public static IServiceCollection AddWeatherForecastTransport(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastRequestHandler, WeatherForecastRequestHandler>();

        return services;
    }
}