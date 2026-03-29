using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;
using Practice.Backend.CurrencyConverter.Infrastructure.Configurations;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddFrankfurterApiClient();

        services.Configure<CacheConfiguration>(configuration.GetRequiredSection(nameof(CacheConfiguration)));

        services.AddTransient<IExchangeRateProviderFactory, ExchangeRateProviderFactory>();

        services.AddTransient<IExchangeRateSnapshotProvider, ExchangeRateSnapshotProvider>();
        services.Decorate<IExchangeRateSnapshotProvider, CachedExchangeRateSnapshotProvider>();
        services.AddTransient<IExchangeRateProvider, FrankfurterExchangeRateProvider>();

        services.AddTransient<IExchangeRateSnapshotProviderFactory, ExchangeRateSnapshotProviderFactory>();

        var redis = configuration.GetRequiredSection(nameof(Redis)).Get<Redis>()!;

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redis.ConnectionString;
            options.InstanceName = redis.InstanceName;
        });

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(redis.ConnectionString));
    }
}
