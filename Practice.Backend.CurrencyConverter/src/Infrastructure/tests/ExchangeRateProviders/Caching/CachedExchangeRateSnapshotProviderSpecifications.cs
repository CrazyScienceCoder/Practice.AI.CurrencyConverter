using ErrorOr;
using Microsoft.Extensions.Caching.Distributed;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public sealed partial class CachedExchangeRateSnapshotProviderSpecifications
{
    private static readonly Currency BaseCurrency = Currency.Create("EUR");
    private static readonly ExchangeDate From = ExchangeDate.Create(new DateOnly(2024, 1, 1));
    private static readonly ExchangeDate To = ExchangeDate.Create(new DateOnly(2024, 1, 15));

    private static ExchangeRateSnapshot BuildSnapshot() => new()
    {
        Base = Currency.Create("EUR"),
        Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
        Rates = new()
        {
            { Currency.Create("USD"), Amount.Create(1.08m) }
        }
    };

    private static HistoricalExchangeRateSnapshot BuildHistoricalSnapshot() => new()
    {
        Amount = Amount.Create(1m),
        Base = Currency.Create("EUR"),
        StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
        EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 14)),
        Rates = new()
        {
            {
                ExchangeDate.Create(new DateOnly(2024, 1, 1)),
                new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
            }
        }
    };

    [Fact]
    public void Provider_Always_DelegatesToInner()
    {
        var builder = new TestBuilder();
        var sut = builder.Build();

        var result = sut.Provider;

        result.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public async Task GetLatestAsync_CacheHit_ReturnsCachedSnapshotWithoutCallingInner()
    {
        var snapshot = BuildSnapshot();
        var builder = new TestBuilder().WithCachedSnapshot(snapshot);
        var sut = builder.Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        result.Value.Base.Value.Should().Be(snapshot.Base.Value);
        builder.InnerMock.Verify(
            p => p.GetLatestAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLatestAsync_CacheHit_DoesNotWriteToCache()
    {
        var snapshot = BuildSnapshot();
        var builder = new TestBuilder().WithCachedSnapshot(snapshot);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLatestAsync_CacheMissAndInnerSucceeds_ReturnsInnerResult()
    {
        var snapshot = BuildSnapshot();
        ErrorOr<ExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        result.Value.Base.Value.Should().Be(snapshot.Base.Value);
    }

    [Fact]
    public async Task GetLatestAsync_CacheMissAndInnerSucceeds_WritesResultToCache()
    {
        var snapshot = BuildSnapshot();
        ErrorOr<ExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_CacheMissAndInnerSucceeds_CallsInnerOnce()
    {
        var snapshot = BuildSnapshot();
        ErrorOr<ExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.InnerMock.Verify(
            p => p.GetLatestAsync(BaseCurrency, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_CacheMissAndInnerReturnsError_PropagatesError()
    {
        ErrorOr<ExchangeRateSnapshot> innerResult = Error.NotFound();
        var builder = new TestBuilder().WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        var result = await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetLatestAsync_CacheMissAndInnerReturnsError_DoesNotWriteToCache()
    {
        ErrorOr<ExchangeRateSnapshot> innerResult = Error.NotFound();
        var builder = new TestBuilder().WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetLatestAsync_CacheHitWithStaleDate_CallsInner()
    {
        var snapshot = BuildSnapshot();
        var staleTime = new DateTimeOffset(2024, 1, 16, 17, 0, 0, TimeSpan.Zero);
        ErrorOr<ExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder()
            .WithCachedSnapshot(snapshot)
            .WithCurrentTime(staleTime)
            .WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.InnerMock.Verify(
            p => p.GetLatestAsync(BaseCurrency, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetLatestAsync_CacheHitWithStaleDate_WritesNewResultToCache()
    {
        var snapshot = BuildSnapshot();
        var staleTime = new DateTimeOffset(2024, 1, 16, 17, 0, 0, TimeSpan.Zero);
        ErrorOr<ExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder()
            .WithCachedSnapshot(snapshot)
            .WithCurrentTime(staleTime)
            .WithInnerLatestResult(innerResult);
        var sut = builder.Build();

        await sut.GetLatestAsync(BaseCurrency, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheHit_ReturnsCachedSnapshotWithoutCallingInner()
    {
        var snapshot = BuildHistoricalSnapshot();
        var builder = new TestBuilder().WithCachedHistoricalSnapshot(snapshot);
        var sut = builder.Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        result.Value.Base.Value.Should().Be(snapshot.Base.Value);
        builder.InnerMock.Verify(
            p => p.GetTimeSeriesAsync(
                It.IsAny<Currency>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheHit_DoesNotWriteToCache()
    {
        var snapshot = BuildHistoricalSnapshot();
        var builder = new TestBuilder().WithCachedHistoricalSnapshot(snapshot);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheMissAndInnerSucceeds_ReturnsInnerResult()
    {
        var snapshot = BuildHistoricalSnapshot();
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        result.Value.Base.Value.Should().Be(snapshot.Base.Value);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheMissAndInnerSucceeds_WritesResultToCache()
    {
        var snapshot = BuildHistoricalSnapshot();
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheMissAndInnerSucceeds_CallsInnerOnce()
    {
        var snapshot = BuildHistoricalSnapshot();
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder().WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.InnerMock.Verify(
            p => p.GetTimeSeriesAsync(
                BaseCurrency,
                From,
                To,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheMissAndInnerReturnsError_PropagatesError()
    {
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = Error.NotFound();
        var builder = new TestBuilder().WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheMissAndInnerReturnsError_DoesNotWriteToCache()
    {
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = Error.NotFound();
        var builder = new TestBuilder().WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheHitWithEndDateInPast_ReturnsCachedWithoutCallingInner()
    {
        var snapshot = BuildHistoricalSnapshot(); // EndDate: 2024-01-14 < 2024-01-15 (today)
        var builder = new TestBuilder().WithCachedHistoricalSnapshot(snapshot);
        var sut = builder.Build();

        var result = await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        builder.InnerMock.Verify(
            p => p.GetTimeSeriesAsync(
                It.IsAny<Currency>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheHitWithEndDateTodayAndMissingExpectedRate_CallsInner()
    {
        // EndDate == today (2024-01-16), rates missing entry for 2024-01-16 → stale
        var staleTime = new DateTimeOffset(2024, 1, 16, 17, 0, 0, TimeSpan.Zero);
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 16)),
            Rates = new()
            {
                {
                    ExchangeDate.Create(new DateOnly(2024, 1, 15)),
                    new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
                }
            }
        };
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder()
            .WithCachedHistoricalSnapshot(snapshot)
            .WithCurrentTime(staleTime)
            .WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.InnerMock.Verify(
            p => p.GetTimeSeriesAsync(
                It.IsAny<Currency>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<ExchangeDate>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_CacheHitWithEndDateTodayAndMissingExpectedRate_WritesNewResultToCache()
    {
        var staleTime = new DateTimeOffset(2024, 1, 16, 17, 0, 0, TimeSpan.Zero);
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 16)),
            Rates = new()
            {
                {
                    ExchangeDate.Create(new DateOnly(2024, 1, 15)),
                    new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
                }
            }
        };
        ErrorOr<HistoricalExchangeRateSnapshot> innerResult = snapshot;
        var builder = new TestBuilder()
            .WithCachedHistoricalSnapshot(snapshot)
            .WithCurrentTime(staleTime)
            .WithInnerTimeSeriesResult(innerResult);
        var sut = builder.Build();

        await sut.GetTimeSeriesAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
