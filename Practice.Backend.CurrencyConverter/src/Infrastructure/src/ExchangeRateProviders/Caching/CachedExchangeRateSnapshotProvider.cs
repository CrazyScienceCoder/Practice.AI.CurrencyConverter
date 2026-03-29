using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.Configurations;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;
using Practice.Backend.CurrencyConverter.Infrastructure.Extensions;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

public sealed class CachedExchangeRateSnapshotProvider(
    IExchangeRateSnapshotProvider inner,
    IDistributedCache cache,
    IOptions<CacheConfiguration> cacheOptions,
    TimeProvider timeProvider,
    ILogger<CachedExchangeRateSnapshotProvider> logger)
    : IExchangeRateSnapshotProvider
{
    public ExchangeRateProvider Provider => inner.Provider;

    public async Task<ErrorOr<ExchangeRateSnapshot>> GetLatestAsync(Currency baseCurrency
        , CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Latest(baseCurrency, Provider);

        var cached = await cache.GetAsync<ExchangeRateSnapshot>(cacheKey, logger, cancellationToken);
        if (cached is not null && IsLatestSnapshotCurrent(cached))
        {
            return cached;
        }

        var result = await inner.GetLatestAsync(baseCurrency, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheOptions.Value.LatestRatesTtl };
        await cache.SetAsync(cacheKey, result.Value, options, logger, cancellationToken);

        return result;
    }

    public async Task<ErrorOr<HistoricalExchangeRateSnapshot>> GetTimeSeriesAsync(Currency baseCurrency
        , ExchangeDate from
        , ExchangeDate to
        , CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeys.Historical(baseCurrency, from, to, Provider);

        var cached = await cache.GetAsync<HistoricalExchangeRateSnapshot>(cacheKey, logger, cancellationToken);
        if (cached is not null && IsTimeSeriesSnapshotCurrent(cached))
        {
            return cached;
        }

        var result = await inner.GetTimeSeriesAsync(baseCurrency, from, to, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheOptions.Value.HistoricalRatesTtl };
        await cache.SetAsync(cacheKey, result.Value, options, logger, cancellationToken);

        return result;
    }

    private bool IsLatestSnapshotCurrent(ExchangeRateSnapshot snapshot)
    {
        var expectedDate = FrankfurterUpdateSchedule.GetExpectedTradingDate(timeProvider.GetUtcNow());
        return snapshot.Date.Value == expectedDate;
    }

    private bool IsTimeSeriesSnapshotCurrent(HistoricalExchangeRateSnapshot snapshot)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        if (snapshot.EndDate.Value < today)
        {
            return true;
        }

        var expectedDate = FrankfurterUpdateSchedule.GetExpectedTradingDate(timeProvider.GetUtcNow());
        return snapshot.Rates.ContainsKey(ExchangeDate.Create(expectedDate));
    }
}
