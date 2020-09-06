using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JOS.Throttler.Benchmark
{
    public class GetWeatherForecastQuerySequentially : IGetWeatherForecastQuery
    {
        private readonly DummyApiHttpClient _dummyApiHttpClient;

        public GetWeatherForecastQuerySequentially(DummyApiHttpClient dummyApiHttpClient)
        {
            _dummyApiHttpClient = dummyApiHttpClient ?? throw new ArgumentNullException(nameof(dummyApiHttpClient));
        }

        public async Task<IReadOnlyCollection<WeatherForecastResponse>> Execute(IReadOnlyList<string> locations)
        {
            var responses = new List<IReadOnlyCollection<WeatherForecastResponse>>(locations.Count);

            for (var i = 0; i < locations.Count; i++)
            {
                var result = await _dummyApiHttpClient.GetWeatherForecast(locations[i]);
                responses.Add(result);
            }

            return responses.SelectMany(x => x).ToArray();
        }
    }
}
