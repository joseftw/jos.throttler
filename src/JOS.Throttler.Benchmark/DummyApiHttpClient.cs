using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JOS.Throttler.Benchmark
{
    public class DummyApiHttpClient
    {
        private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General)
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public DummyApiHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyCollection<WeatherForecastResponse>> GetWeatherForecast(string location)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/weatherforecast?location={location}");
            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            await using var responseContent = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<IReadOnlyCollection<WeatherForecastResponse>>(responseContent, DefaultJsonSerializerOptions);
        }
    }
}
