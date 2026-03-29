using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.ExchangeRates;

public sealed class ExchangeRateSpecifications
{
    [Fact]
    public void Constructor_RequiredPropertiesSet_StoresCorrectValues()
    {
        var rate = BuildExchangeRate();

        rate.Amount.Should().Be(new Amount(1m));
        rate.Base.Should().Be(new Currency("USD"));
        rate.Date.Should().Be(ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)));
    }

    [Fact]
    public void Constructor_NoRatesProvided_RatesIsEmpty()
    {
        var rate = BuildExchangeRate();

        rate.Rates.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_RatesProvided_ContainsThoseRates()
    {
        var rates = new Dictionary<Currency, Amount>
        {
            [new Currency("EUR")] = new(0.92m),
            [new Currency("GBP")] = new(0.79m)
        };

        var rate = BuildExchangeRate(rates);

        rate.Rates.Should().HaveCount(2);
        rate.Rates[new Currency("EUR")].Value.Should().Be(0.92m);
        rate.Rates[new Currency("GBP")].Value.Should().Be(0.79m);
    }

    [Fact]
    public void Equals_SameProperties_ReturnsTrue()
    {
        var sharedRates = new Dictionary<Currency, Amount>();

        var r1 = BuildExchangeRate(sharedRates);
        var r2 = BuildExchangeRate(sharedRates);

        r1.Should().Be(r2);
    }

    [Fact]
    public void Equals_DifferentAmounts_ReturnsFalse()
    {
        var r1 = BuildExchangeRate(amount: new Amount(1m));
        var r2 = BuildExchangeRate(amount: new Amount(10m));

        r1.Should().NotBe(r2);
    }

    private static ExchangeRate BuildExchangeRate(
        Dictionary<Currency, Amount>? rates = null,
        Amount? amount = null)
        => new()
        {
            Amount = amount ?? new Amount(1m),
            Base = new Currency("USD"),
            Date = ExchangeDate.Create(DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1)),
            Rates = rates ?? []
        };
}
