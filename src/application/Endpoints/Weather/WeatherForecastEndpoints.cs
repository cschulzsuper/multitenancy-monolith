using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Weather
{
    public static class WeatherForecastEndpoints
    {
        public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var wheatherForecastEndpoints = endpoints
                .MapGroup("/wheather-forecast")
                .RequireAuthorization();

            wheatherForecastEndpoints
                .MapGet(string.Empty, GetAll);

            return endpoints;
        }

        private static Delegate GetAll =>
            (IWeatherForecastRequestHandler requestHandler)
                => requestHandler.GetAll();
    }
}
