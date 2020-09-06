using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace JOS.Throttler.Benchmark
{
    public class GetWeatherForecastQueryTransformBlock : IGetWeatherForecastQuery
    {
        private readonly DummyApiHttpClient _dummyApiHttpClient;

        public GetWeatherForecastQueryTransformBlock(DummyApiHttpClient dummyApiHttpClient)
        {
            _dummyApiHttpClient = dummyApiHttpClient ?? throw new ArgumentNullException(nameof(dummyApiHttpClient));
        }

        public async Task<IReadOnlyCollection<WeatherForecastResponse>> Execute(IReadOnlyList<string> locations)
        {
            var transformBlock = new TransformBlock<string, IReadOnlyCollection<WeatherForecastResponse>>(
                _dummyApiHttpClient.GetWeatherForecast,
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = ThrottlerBenchmark.MaxConcurrency
                }
            );

            var buffer = new BufferBlock<IReadOnlyCollection<WeatherForecastResponse>>();
            transformBlock.LinkTo(buffer);
            for (var i = 0; i < locations.Count; i++)
            {
                await transformBlock.SendAsync(locations[i]);
            }

            transformBlock.Complete();
            await transformBlock.Completion;

            return buffer.TryReceiveAll(out var forecasts)
                ? forecasts.SelectMany(x => x).ToArray()
                : throw new Exception("Error when trying to receive items from Buffer");
        }
    }
}
