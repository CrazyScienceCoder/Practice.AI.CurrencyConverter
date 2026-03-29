using System.Text;
using System.Text.Json;
using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.Configurations;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public partial class CachedExchangeRateSnapshotProviderSpecifications
{
    private class TestBuilder
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            Converters =
            {
                new CurrencyJsonConverter(),
                new ExchangeDateJsonConverter(),
                new AmountJsonConverter()
            }
        };

        private static readonly DateTimeOffset DefaultUtcNow =
            new(2024, 1, 15, 17, 0, 0, TimeSpan.Zero);

        public Mock<IExchangeRateSnapshotProvider> InnerMock { get; } = new();

        public Mock<IDistributedCache> CacheMock { get; } = new();

        private Mock<TimeProvider> TimeProviderMock { get; } = new();

        private readonly Mock<ILogger<CachedExchangeRateSnapshotProvider>> _loggerMock = new();

        private readonly IOptions<CacheConfiguration> _cacheOptions = Options.Create(new CacheConfiguration());

        public TestBuilder()
        {
            InnerMock.Setup(p => p.Provider).Returns(ExchangeRateProvider.Frankfurter);

            TimeProviderMock.Setup(t => t.GetUtcNow()).Returns(DefaultUtcNow);

            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[]?)null);

            CacheMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public TestBuilder WithCachedSnapshot(ExchangeRateSnapshot snapshot)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapshot, SerializerOptions));
            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);
            return this;
        }

        public TestBuilder WithCachedHistoricalSnapshot(HistoricalExchangeRateSnapshot snapshot)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(snapshot, SerializerOptions));
            CacheMock
                .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(bytes);
            return this;
        }

        public TestBuilder WithInnerLatestResult(ErrorOr<ExchangeRateSnapshot> result)
        {
            InnerMock
                .Setup(p => p.GetLatestAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public TestBuilder WithInnerTimeSeriesResult(ErrorOr<HistoricalExchangeRateSnapshot> result)
        {
            InnerMock
                .Setup(p => p.GetTimeSeriesAsync(
                    It.IsAny<Currency>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<ExchangeDate>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);
            return this;
        }

        public TestBuilder WithCurrentTime(DateTimeOffset utcNow)
        {
            TimeProviderMock.Setup(t => t.GetUtcNow()).Returns(utcNow);
            return this;
        }

        public CachedExchangeRateSnapshotProvider Build()
            => new(InnerMock.Object, CacheMock.Object, _cacheOptions, TimeProviderMock.Object, _loggerMock.Object);
    }
}
