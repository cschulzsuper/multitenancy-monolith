using ChristianSchulz.MultitenancyMonolith.Application.Weather.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ChristianSchulz.MultitenancyMonolith.Application.Weather
{
    public interface IWeatherForecastRequestHandler
    {
        IEnumerable<WeatherForecastResponse> GetAll();
    }
}
