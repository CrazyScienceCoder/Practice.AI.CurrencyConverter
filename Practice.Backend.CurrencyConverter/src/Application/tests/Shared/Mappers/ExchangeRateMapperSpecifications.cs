using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.Shared.Mappers;

public sealed class ExchangeRateMapperSpecifications
{
    [Fact]
    public void FilterExcludedCurrencies_WithNoForbiddenCurrencies_ReturnsAllRates()
    {
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) },
            { new Currency("GBP"), new Amount(0.78m) }
        });

        var result = exchangeRate.FilterExcludedCurrencies();

        result.Rates.Should().HaveCount(2);
        result.Rates.Should().ContainKey(new Currency("EUR"));
        result.Rates.Should().ContainKey(new Currency("GBP"));
    }

    [Fact]
    public void FilterExcludedCurrencies_WithForbiddenCurrencies_RemovesForbiddenOnesOnly()
    {
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) },
            { new Currency("MXN"), new Amount(17.5m) },
            { new Currency("PLN"), new Amount(4.1m) },
            { new Currency("THB"), new Amount(35m) },
            { new Currency("TRY"), new Amount(32m) }
        });

        var result = exchangeRate.FilterExcludedCurrencies();

        result.Rates.Should().HaveCount(1);
        result.Rates.Should().ContainKey(new Currency("EUR"));
        result.Rates.Should().NotContainKey(new Currency("MXN"));
        result.Rates.Should().NotContainKey(new Currency("PLN"));
        result.Rates.Should().NotContainKey(new Currency("THB"));
        result.Rates.Should().NotContainKey(new Currency("TRY"));
    }

    [Fact]
    public void FilterExcludedCurrencies_WithAllForbiddenCurrencies_ReturnsEmptyRates()
    {
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("MXN"), new Amount(17.5m) },
            { new Currency("PLN"), new Amount(4.1m) }
        });

        var result = exchangeRate.FilterExcludedCurrencies();

        result.Rates.Should().BeEmpty();
    }

    [Fact]
    public void FilterExcludedCurrencies_PreservesOriginalExchangeRateProperties()
    {
        var date = new DateOnly(2025, 1, 15);
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) }
        }, date);

        var result = exchangeRate.FilterExcludedCurrencies();

        result.Amount.Value.Should().Be(1m);
        result.Base.Value.Should().Be("USD");
        result.Date.Value.Should().Be(date);
    }

    [Fact]
    public void ToLatestExchangeRateResult_ValidExchangeRate_MapsAllPropertiesCorrectly()
    {
        var date = new DateOnly(2025, 1, 15);
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) },
            { new Currency("GBP"), new Amount(0.78m) }
        }, date);

        var result = exchangeRate.ToLatestExchangeRateResult();

        result.Should().NotBeNull();
        result.Amount.Should().Be(1m);
        result.Base.Should().Be("USD");
        result.Date.Should().Be(date);
        result.Rates.Should().HaveCount(2);
        result.Rates.Should().ContainKey("EUR").WhoseValue.Should().Be(0.92m);
        result.Rates.Should().ContainKey("GBP").WhoseValue.Should().Be(0.78m);
    }

    [Fact]
    public void ToLatestExchangeRateResult_EmptyRates_ReturnsEmptyRatesDictionary()
    {
        var exchangeRate = BuildExchangeRate(new Dictionary<Currency, Amount>());

        var result = exchangeRate.ToLatestExchangeRateResult();

        result.Rates.Should().BeEmpty();
    }

    private static ExchangeRate BuildExchangeRate(
        Dictionary<Currency, Amount> rates,
        DateOnly? date = null)
        => new()
        {
            Amount = new Amount(1m),
            Base = new Currency("USD"),
            Date = new ExchangeDate(date ?? new DateOnly(2025, 1, 15)),
            Rates = rates
        };
}
