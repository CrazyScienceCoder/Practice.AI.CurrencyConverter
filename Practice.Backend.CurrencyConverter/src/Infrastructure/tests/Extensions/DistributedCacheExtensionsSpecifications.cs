using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;
using Practice.Backend.CurrencyConverter.Infrastructure.Extensions;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.Extensions;

public sealed class DistributedCacheExtensionsSpecifications
{
    // Mirrors the private Options inside DistributedCacheExtensions
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new CurrencyJsonConverter(),
            new ExchangeDateJsonConverter(),
            new AmountJsonConverter()
        }
    };

    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly Mock<ILogger> _loggerMock = new();

    private static ExchangeRateSnapshot BuildSnapshot() => new()
    {
        Base = Currency.Create("EUR"),
        Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
        Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
    };

    private byte[] Serialize<T>(T value)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, SerializerOptions));

    [Fact]
    public async Task GetAsync_CacheReturnsSerializedValue_ReturnsDeserializedObject()
    {
        var snapshot = BuildSnapshot();
        _cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Serialize(snapshot));

        var result = await _cacheMock.Object.GetAsync<ExchangeRateSnapshot>("key", _loggerMock.Object, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Base.Value.Should().Be(snapshot.Base.Value);
    }

    [Fact]
    public async Task GetAsync_CacheReturnsSerializedValue_ReturnsObjectWithCorrectRates()
    {
        var snapshot = BuildSnapshot();
        _cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Serialize(snapshot));

        var result = await _cacheMock.Object.GetAsync<ExchangeRateSnapshot>("key", _loggerMock.Object, TestContext.Current.CancellationToken);

        result!.Rates.Should().ContainKey(Currency.Create("USD"));
        result.Rates[Currency.Create("USD")].Value.Should().Be(1.08m);
    }

    [Fact]
    public async Task GetAsync_CacheReturnsNull_ReturnsNull()
    {
        _cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await _cacheMock.Object.GetAsync<ExchangeRateSnapshot>("key", _loggerMock.Object, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_CacheThrowsException_ReturnsNull()
    {
        _cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache unavailable"));

        var result = await _cacheMock.Object.GetAsync<ExchangeRateSnapshot>("key", _loggerMock.Object, TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAsync_CacheThrowsException_DoesNotPropagateException()
    {
        _cacheMock
            .Setup(c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache unavailable"));

        var act = async () => await _cacheMock.Object.GetAsync<ExchangeRateSnapshot>("key", _loggerMock.Object, TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SetAsync_ValidValue_CallsUnderlyingCacheSetWithSerializedBytes()
    {
        var snapshot = BuildSnapshot();
        var entryOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) };

        _cacheMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _cacheMock.Object.SetAsync("key", snapshot, entryOptions, _loggerMock.Object, TestContext.Current.CancellationToken);

        _cacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_CacheThrowsException_DoesNotPropagateException()
    {
        var snapshot = BuildSnapshot();
        var entryOptions = new DistributedCacheEntryOptions();

        _cacheMock
            .Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache unavailable"));

        var act = async () => await _cacheMock.Object.SetAsync("key", snapshot, entryOptions, _loggerMock.Object, TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync();
    }
}
