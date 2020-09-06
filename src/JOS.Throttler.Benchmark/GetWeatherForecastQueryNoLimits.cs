using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JOS.Throttler.Benchmark
{
    public class GetWeatherForecastQueryNoLimits : IGetWeatherForecastQuery
    {
        private readonly DummyApiHttpClient _dummyApiHttpClient;

        public GetWeatherForecastQueryNoLimits(DummyApiHttpClient dummyApiHttpClient)
        {
            _dummyApiHttpClient = dummyApiHttpClient ?? throw new ArgumentNullException(nameof(dummyApiHttpClient));
        }

        public async Task<IReadOnlyCollection<WeatherForecastResponse>> Execute(IReadOnlyList<string> locations)
        {
            var responses = new ConcurrentBag<IReadOnlyCollection<WeatherForecastResponse>>();
            var tasks = locations.Select(async location =>
            {
                var response = await _dummyApiHttpClient.GetWeatherForecast(location);
                responses.Add(response);
            });
            await Task.WhenAll(tasks);

            return responses.SelectMany(x => x).ToArray();
        }
    }
}
