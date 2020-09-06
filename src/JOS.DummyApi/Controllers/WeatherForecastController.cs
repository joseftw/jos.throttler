using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JOS.DummyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly List<WeatherForecast> Forecasts = new List<WeatherForecast>
        {
            new WeatherForecast
            {
                Date = DateTime.UtcNow.Date,
                TemperatureC = 30,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = DateTime.UtcNow.AddDays(1).Date,
                TemperatureC = 30,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = DateTime.UtcNow.AddDays(2).Date,
                TemperatureC = 30,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = DateTime.UtcNow.AddDays(3).Date,
                TemperatureC = 30,
                Summary = "Freezing"
            },
            new WeatherForecast
            {
                Date = DateTime.UtcNow.AddDays(4).Date,
                TemperatureC = 30,
                Summary = "Freezing"
            },
        };
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            await Task.Delay(25);
            return Forecasts;
        }
    }
}
