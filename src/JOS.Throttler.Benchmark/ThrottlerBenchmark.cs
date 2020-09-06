using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace JOS.Throttler.Benchmark
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp50, warmupCount: 1, invocationCount: 1, launchCount:1, targetCount: 50)]
    public class ThrottlerBenchmark
    {
        public const int MaxConcurrency = 10;
        private const int LocationsToCreate = 10000;
        private IServiceProvider _serviceProvider;
        private static IReadOnlyList<string> _locations;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var servicesCollection = new ServiceCollection();
            var services = new DefaultServiceProviderFactory().CreateBuilder(servicesCollection);
            services.AddHttpClient<DummyApiHttpClient>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri("http://localhost:5000");
            });
            services.AddTransient<GetWeatherForecastQueryTransformBlock>();
            services.AddTransient<GetWeatherForecastQueryNoLimits>();
            services.AddTransient<GetWeatherForecastQuerySemaphoreSlim>();
            services.AddTransient<GetWeatherForecastQuerySequentially>();
            _serviceProvider = services.BuildServiceProvider();

            _locations = Enumerable.Range(0, LocationsToCreate).Select(x => $"location-{x}").ToList();
        }

        [Benchmark(Baseline = true)]
        public async Task<IReadOnlyCollection<WeatherForecastResponse>> Sequentially()
        {
            var query = _serviceProvider.GetRequiredService<GetWeatherForecastQuerySequentially>();
            return await query.Execute(_locations);
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<WeatherForecastResponse>> TransformBlock()
        {
            var query = _serviceProvider.GetRequiredService<GetWeatherForecastQueryTransformBlock>();
            return await query.Execute(_locations);
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<WeatherForecastResponse>> SemaphoreSlim()
        {
            var query = _serviceProvider.GetRequiredService<GetWeatherForecastQuerySemaphoreSlim>();
            return await query.Execute(_locations);
        }

        [Benchmark]
        public async Task<IReadOnlyCollection<WeatherForecastResponse>> NoLimits()
        {
            var query = _serviceProvider.GetRequiredService<GetWeatherForecastQueryNoLimits>();
            return await query.Execute(_locations);
        }

    }
}
