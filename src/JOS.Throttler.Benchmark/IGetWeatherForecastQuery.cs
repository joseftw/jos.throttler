using System.Collections.Generic;
using System.Threading.Tasks;

namespace JOS.Throttler.Benchmark
{
    public interface IGetWeatherForecastQuery
    {
        Task<IReadOnlyCollection<WeatherForecastResponse>> Execute(IReadOnlyList<string> locations);
    }
}