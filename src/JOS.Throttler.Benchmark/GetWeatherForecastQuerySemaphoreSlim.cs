using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JOS.Throttler.Benchmark
{
    public class GetWeatherForecastQuerySemaphoreSlim : IGetWeatherForecastQuery
    {
        private readonly DummyApiHttpClient _dummyApiHttpClient;

        public GetWeatherForecastQuerySemaphoreSlim(DummyApiHttpClient dummyApiHttpClient)
        {
            _dummyApiHttpClient = dummyApiHttpClient ?? throw new ArgumentNullException(nameof(dummyApiHttpClient));
        }

        public async Task<IReadOnlyCollection<WeatherForecastResponse>> Execute(IReadOnlyList<string> locations)
        {
            var semaphoreSlim = new SemaphoreSlim(
                ThrottlerBenchmark.MaxConcurrency,
                ThrottlerBenchmark.MaxConcurrency);
            var responses = new ConcurrentBag<IReadOnlyCollection<WeatherForecastResponse>>();

            var tasks = locations.Select(async location =>
            {
                await semaphoreSlim.WaitAsync();

                try
                {
                    var response = await _dummyApiHttpClient.GetWeatherForecast(location);
                    responses.Add(response);
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });

            await Task.WhenAll(tasks);
            return responses.SelectMany(x => x).ToArray();
        }
    }
}
