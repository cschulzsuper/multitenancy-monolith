using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public static class WeatherForecastEndpoints
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var wheatherForecastEndpoints = endpoints
            .MapGroup("/wheather-forecast")
            .WithTags("Wheather Forecast API");

        wheatherForecastEndpoints
            .MapGet(string.Empty, GetAll);

        return endpoints;
    }

    private static Delegate GetAll =>
        [Authorize(Roles = "Member")]
        (IWeatherForecastRequestHandler requestHandler)
            => requestHandler.GetAll();
}