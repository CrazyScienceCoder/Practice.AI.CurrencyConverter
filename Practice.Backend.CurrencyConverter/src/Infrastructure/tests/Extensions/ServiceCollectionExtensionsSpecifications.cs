using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;
using Practice.Backend.CurrencyConverter.Infrastructure.Extensions;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.Extensions;

public sealed class ServiceCollectionExtensionsSpecifications
{
    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CacheConfiguration:LatestRatesTtl"] = "01:00:00",
                ["CacheConfiguration:HistoricalRatesTtl"] = "30.00:00:00",
                ["Redis:ConnectionString"] = "localhost:6379",
                ["Redis:InstanceName"] = "test"
            })
            .Build();

    [Fact]
    public void AddInfrastructure_Always_RegistersTimeProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d => d.ServiceType == typeof(TimeProvider));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersTimeProviderAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        var descriptor = services.First(d => d.ServiceType == typeof(TimeProvider));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersExchangeRateProviderFactory()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d =>
            d.ServiceType == typeof(IExchangeRateProviderFactory) &&
            d.ImplementationType == typeof(ExchangeRateProviderFactory));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersExchangeRateProviderFactoryAsTransient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        var descriptor = services.First(d => d.ServiceType == typeof(IExchangeRateProviderFactory));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersIExchangeRateSnapshotProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d => d.ServiceType == typeof(IExchangeRateSnapshotProvider));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersIExchangeRateProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d =>
            d.ServiceType == typeof(IExchangeRateProvider) &&
            d.ImplementationType == typeof(FrankfurterExchangeRateProvider));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersIExchangeRateProviderAsTransient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        var descriptor = services.First(d => d.ServiceType == typeof(IExchangeRateProvider));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersExchangeRateSnapshotProviderFactory()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d =>
            d.ServiceType == typeof(IExchangeRateSnapshotProviderFactory) &&
            d.ImplementationType == typeof(ExchangeRateSnapshotProviderFactory));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersExchangeRateSnapshotProviderFactoryAsTransient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        var descriptor = services.First(d => d.ServiceType == typeof(IExchangeRateSnapshotProviderFactory));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersDistributedCache()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d => d.ServiceType == typeof(IDistributedCache));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersConnectionMultiplexer()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        services.Should().Contain(d => d.ServiceType == typeof(IConnectionMultiplexer));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersConnectionMultiplexerAsSingleton()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure(BuildConfiguration());

        var descriptor = services.First(d => d.ServiceType == typeof(IConnectionMultiplexer));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddInfrastructure_Always_ConfiguresRedisCacheWithConnectionString()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(BuildConfiguration());

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

        options.Configuration.Should().Be("localhost:6379");
    }

    [Fact]
    public void AddInfrastructure_Always_ConfiguresRedisCacheWithInstanceName()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddInfrastructure(BuildConfiguration());

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

        options.InstanceName.Should().Be("test");
    }

    [Fact]
    public void AddInfrastructure_Always_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var act = () => services.AddInfrastructure(BuildConfiguration());

        act.Should().NotThrow();
    }
}