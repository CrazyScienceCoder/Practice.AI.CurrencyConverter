namespace Practice.Backend.CurrencyConverter.Infrastructure.Configurations;

public sealed class CacheConfiguration
{
    public TimeSpan LatestRatesTtl { get; set; } = TimeSpan.FromHours(24);

    public TimeSpan HistoricalRatesTtl { get; set; } = TimeSpan.FromDays(30);
}
