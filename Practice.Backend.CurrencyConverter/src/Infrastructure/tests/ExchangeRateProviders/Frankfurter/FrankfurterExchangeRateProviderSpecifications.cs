using ErrorOr;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Frankfurter;

public sealed partial class FrankfurterExchangeRateProviderSpecifications
{
    private static readonly Currency BaseCurrency = Currency.Create("EUR");
    private static readonly Currency ToCurrency = Currency.Create("USD");
    private static readonly Amount ConversionAmount = Amount.Create(100m);
    private static readonly ExchangeDate From = ExchangeDate.Create(new DateOnly(2024, 1, 1));
    private static readonly ExchangeDate To = ExchangeDate.Create(new DateOnly(2024, 1, 15));

    private static ExchangeRateSnapshot BuildSnapshot() => new()
    {
        Base = Currency.Create("EUR"),
        Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
        Rates = new()
        {
            { Currency.Create("USD"), Amount.Create(1.08m) },
            { Currency.Create("GBP"), Amount.Create(0.86m) }
        }
    };

    private static HistoricalExchangeRateSnapshot BuildHistoricalSnapshot() => new()
    {
        Amount = Amount.Create(1m),
        Base = Currency.Create("EUR"),
        StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
        EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
        Rates = new()
        {
            {
                ExchangeDate.Create(new DateOnly(2024, 1, 1)),
                new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
            }
        }
    };

    [Fact]
    public void Provider_Always_ReturnsFrankfurter()
    {
        var sut = new TestBuilder().Build();

        sut.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public async Task ConvertAsync_SnapshotProviderReturnsSnapshot_ReturnsExchangeRateWithFilteredCurrency()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = BuildSnapshot();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.ConvertAsync(BaseCurrency, ToCurrency, ConversionAmount, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
        result.Value.Rates.Should().ContainKey(ToCurrency);
    }

    [Fact]
    public async Task ConvertAsync_SnapshotProviderReturnsSnapshot_RateReflectsConversionAmount()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = BuildSnapshot();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.ConvertAsync(BaseCurrency, ToCurrency, ConversionAmount, TestContext.Current.CancellationToken);

        result.Value.Amount.Value.Should().Be(ConversionAmount.Value);
    }

    [Fact]
    public async Task ConvertAsync_SnapshotProviderReturnsError_PropagatesFirstError()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = Error.NotFound();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.ConvertAsync(BaseCurrency, ToCurrency, ConversionAmount, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetLatestExchangeRateAsync_SnapshotProviderReturnsSnapshot_ReturnsExchangeRate()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = BuildSnapshot();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.GetLatestExchangeRateAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetLatestExchangeRateAsync_SnapshotProviderReturnsSnapshot_RateAmountIsOne()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = BuildSnapshot();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.GetLatestExchangeRateAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.Value.Amount.Value.Should().Be(1m);
    }

    [Fact]
    public async Task GetLatestExchangeRateAsync_SnapshotProviderReturnsSnapshot_ContainsAllRates()
    {
        var snapshot = BuildSnapshot();
        ErrorOr<ExchangeRateSnapshot> snapshotResult = snapshot;
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.GetLatestExchangeRateAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.Value.Rates.Should().HaveCount(snapshot.Rates.Count);
    }

    [Fact]
    public async Task GetLatestExchangeRateAsync_SnapshotProviderReturnsError_PropagatesFirstError()
    {
        ErrorOr<ExchangeRateSnapshot> snapshotResult = Error.Unexpected();
        var sut = new TestBuilder().WithLatestResult(snapshotResult).Build();

        var result = await sut.GetLatestExchangeRateAsync(BaseCurrency, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Unexpected);
    }

    [Fact]
    public async Task GetHistoricalExchangeRateAsync_SnapshotProviderReturnsSnapshot_ReturnsHistoricalRate()
    {
        ErrorOr<HistoricalExchangeRateSnapshot> snapshotResult = BuildHistoricalSnapshot();
        var sut = new TestBuilder().WithTimeSeriesResult(snapshotResult).Build();

        var result = await sut.GetHistoricalExchangeRateAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeFalse();
    }

    [Fact]
    public async Task GetHistoricalExchangeRateAsync_SnapshotProviderReturnsSnapshot_HasCorrectDateRange()
    {
        var snapshot = BuildHistoricalSnapshot();
        ErrorOr<HistoricalExchangeRateSnapshot> snapshotResult = snapshot;
        var sut = new TestBuilder().WithTimeSeriesResult(snapshotResult).Build();

        var result = await sut.GetHistoricalExchangeRateAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.Value.StartDate.Value.Should().Be(snapshot.StartDate.Value);
        result.Value.EndDate.Value.Should().Be(snapshot.EndDate.Value);
    }

    [Fact]
    public async Task GetHistoricalExchangeRateAsync_SnapshotProviderReturnsSnapshot_HasCorrectBase()
    {
        var snapshot = BuildHistoricalSnapshot();
        ErrorOr<HistoricalExchangeRateSnapshot> snapshotResult = snapshot;
        var sut = new TestBuilder().WithTimeSeriesResult(snapshotResult).Build();

        var result = await sut.GetHistoricalExchangeRateAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.Value.Base.Value.Should().Be(snapshot.Base.Value);
    }

    [Fact]
    public async Task GetHistoricalExchangeRateAsync_SnapshotProviderReturnsError_PropagatesFirstError()
    {
        ErrorOr<HistoricalExchangeRateSnapshot> snapshotResult = Error.NotFound();
        var sut = new TestBuilder().WithTimeSeriesResult(snapshotResult).Build();

        var result = await sut.GetHistoricalExchangeRateAsync(BaseCurrency, From, To, TestContext.Current.CancellationToken);

        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.NotFound);
    }
}
