## Cache data with Redis

A simple implementation of caching data using Redis

How to

1. Pull and run latest redis docker container - `docker run --name redis -p 5002:6379 -d redis`
2. Install the [packages](#packages)
3. Add *StackExchangeRedisCache* service to `IServiceCollection`
```C#
      services.AddStackExchangeRedisCache(options =>
      {
          options.Configuration = Configuration.GetConnectionString("Redis");
          options.InstanceName = "Redis_"; // any name
      });
```
4. Create an extension of `IDistributedCache` to Set and Get records (Not required).
```C#
      public static class DistributedCache
      {
          public static async Task SetRecordAsync<T>(
              this IDistributedCache cache,
              string recordId,
              T data,
              TimeSpan? absoluteExpireTime = null,
              TimeSpan? unusedExpireTime = null)
          {
              var options = new DistributedCacheEntryOptions
              {
                  AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                  SlidingExpiration = unusedExpireTime
              };

              var jsonData = JsonSerializer.Serialize(data);
              await cache.SetStringAsync(recordId, jsonData, options);
          }

          public static async Task<T> GetRecordAsync<T>(
              this IDistributedCache cache, 
              string recordId)
          {
              var jsonData = await cache.GetStringAsync(recordId);

              return jsonData is null
                  ? default
                  : JsonSerializer.Deserialize<T>(jsonData);
          }
      }
```
5. Create record key
```C#
      var recordKey = $"WeatherForecast_{DateTime.Now.ToString("yyyyMMdd_hhmm")}";
```
6. Add a record to the cache
```C#
      await _cache.SetRecordAsync(recordKey, forecasts);
```
7. Get a record
```C#
      var forecasts = await _cache.GetRecordAsync<IEnumerable<WeatherForecast>>(recordKey);
```

### Packages
- [Microsoft.Extensions.Caching.StackExchangeRedis](https://www.nuget.org/packages/Microsoft.Extensions.Caching.StackExchangeRedis/5.0.0?_src=template)
- [StackExchange.Redis](https://www.nuget.org/packages/StackExchange.Redis/2.1.58?_src=template)

### Links
- https://www.youtube.com/watch?v=UrQWii_kfIE&t=1374s
- https://hub.docker.com/_/redis
- https://github.com/redis/redis