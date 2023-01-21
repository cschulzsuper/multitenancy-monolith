using ChristianSchulz.MultitenancyMonolith.Application.Business.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Business;

public interface IWeatherForecastRequestHandler
{
    IEnumerable<WeatherForecastResponse> GetAll();
}