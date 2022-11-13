using ChristianSchulz.MultitenancyMonolith.Application.Weather.Responses;
using System.Collections.Generic;
namespace ChristianSchulz.MultitenancyMonolith.Application.Weather;

public interface IWeatherForecastRequestHandler
{
    IEnumerable<WeatherForecastResponse> GetAll();
}
