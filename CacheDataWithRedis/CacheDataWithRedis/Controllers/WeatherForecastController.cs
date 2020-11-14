using CacheDataWithRedis.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CacheDataWithRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IDistributedCache _cache;

        public WeatherForecastController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> GetAsync()
        {
            var random = new Random();
            var recordKey = $"WeatherForecast_{DateTime.Now.ToString("yyyyMMdd_hhmm")}";

            var forecasts = await _cache.GetRecordAsync<IEnumerable<WeatherForecast>>(recordKey);

            if (forecasts is null)
            {
                forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = random.Next(-20, 55),
                    Summary = _summaries[random.Next(_summaries.Length)]
                })
                .ToArray();

                await _cache.SetRecordAsync(recordKey, forecasts);
            }


            return forecasts;
        }
    }
}