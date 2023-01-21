using System;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;

public class WeatherForecastResponse
{
    public required DateOnly Date { get; init; }

    public required decimal Temperature { get; init; }

    public required string Summary { get; init; }
}