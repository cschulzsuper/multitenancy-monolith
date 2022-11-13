using ChristianSchulz.MultitenancyMonolith.Application.Weather.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Weather;

public class WeatherForecastRequestHandler : IWeatherForecastRequestHandler
{
    private readonly string[] _availableSummeries = new[]
        { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

    public IEnumerable<WeatherForecastResponse> GetAll()
    {
        var forecast = Enumerable
            .Range(1, 5)
            .Select(x =>
                new WeatherForecastResponse
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(x)),
                    Temperature = Random.Shared.Next(-20, 55),
                    Summary = _availableSummeries[Random.Shared.Next(_availableSummeries.Length)]
                });

        return forecast;
    }
}
