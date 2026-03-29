using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.ExchangeRates;

public sealed class HistoricalExchangeRateSpecifications
{
    [Fact]
    public void Constructor_RequiredPropertiesSet_StoresCorrectValues()
    {
        var historicalRate = BuildHistoricalExchangeRate();

        historicalRate.Amount.Should().Be(new Amount(1m));
        historicalRate.Base.Should().Be(new Currency("USD"));
        historicalRate.StartDate.Should().Be(ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10)));
        historicalRate.EndDate.Should().Be(ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)));
    }

    [Fact]
    public void Constructor_NoRatesProvided_RatesIsEmpty()
    {
        var historicalRate = BuildHistoricalExchangeRate();

        historicalRate.Rates.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_RatesProvided_ContainsThoseRates()
    {
        var startDate = ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10));
        var endDate = ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1));

        var rates = new Dictionary<ExchangeDate, Dictionary<Currency, Amount>>
        {
            [startDate] = new() { [new Currency("EUR")] = new Amount(0.90m) },
            [endDate] = new() { [new Currency("EUR")] = new Amount(0.91m) }
        };

        var historicalRate = BuildHistoricalExchangeRate(rates);

        historicalRate.Rates.Should().HaveCount(2);
        historicalRate.Rates[startDate][new Currency("EUR")].Value.Should().Be(0.90m);
        historicalRate.Rates[endDate][new Currency("EUR")].Value.Should().Be(0.91m);
    }

    [Fact]
    public void Equals_SameProperties_ReturnsTrue()
    {
        var sharedRates = new Dictionary<ExchangeDate, Dictionary<Currency, Amount>>();

        var r1 = BuildHistoricalExchangeRate(sharedRates);
        var r2 = BuildHistoricalExchangeRate(sharedRates);

        r1.Should().Be(r2);
    }

    [Fact]
    public void Equals_DifferentStartDates_ReturnsFalse()
    {
        var earlierStart = ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-20));

        var r1 = BuildHistoricalExchangeRate();
        var r2 = BuildHistoricalExchangeRate(startDate: earlierStart);

        r1.Should().NotBe(r2);
    }

    private static HistoricalExchangeRate BuildHistoricalExchangeRate(
        Dictionary<ExchangeDate, Dictionary<Currency, Amount>>? rates = null,
        ExchangeDate? startDate = null,
        ExchangeDate? endDate = null)
        => new()
        {
            Amount = new Amount(1m),
            Base = new Currency("USD"),
            StartDate = startDate ?? ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10)),
            EndDate = endDate ?? ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)),
            Rates = rates ?? []
        };
}
