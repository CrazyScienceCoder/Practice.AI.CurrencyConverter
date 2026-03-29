using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Extensions;

public static class DistributedCacheExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new CurrencyJsonConverter(),
            new ExchangeDateJsonConverter(),
            new AmountJsonConverter()
        }
    };

    extension(IDistributedCache cache)
    {
        public async Task<T?> GetAsync<T>(string key
            , ILogger logger
            , CancellationToken cancellationToken)
        {
            var result = await ExecuteAsync<T?>(async () =>
            {
                var cached = await cache.GetStringAsync(key, cancellationToken);

                return cached is not null ? JsonSerializer.Deserialize<T>(cached, Options)! : default;
            }, logger);

            return result;
        }

        public async Task SetAsync<T>(string key
            , T value
            , DistributedCacheEntryOptions entryOptions
            , ILogger logger
            , CancellationToken cancellationToken)
        {
            await ExecuteAsync<T?>(async () =>
            {
                await cache.SetStringAsync(key, JsonSerializer.Serialize(value, Options), entryOptions, cancellationToken);
                return default;
            }, logger);
        }
    }

    private static async Task<T?> ExecuteAsync<T>(Func<Task<T?>> func, ILogger logger)
    {
        try
        {
            return await func();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while accessing the distributed cache");
        }

        return default;
    }
}
