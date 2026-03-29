using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.Shared.Mappers;

public sealed class CurrencyConversionMapperSpecifications
{
    [Fact]
    public void ToCurrencyConversionResult_ValidExchangeRate_MapsAllPropertiesCorrectly()
    {
        var exchangeRate = new ExchangeRate
        {
            Amount = new Amount(100m),
            Base = new Currency("USD"),
            Date = new ExchangeDate(new DateOnly(2025, 1, 15)),
            Rates = new Dictionary<Currency, Amount>
            {
                { new Currency("EUR"), new Amount(92m) },
                { new Currency("GBP"), new Amount(78m) }
            }
        };

        var result = exchangeRate.ToCurrencyConversionResult();

        result.Should().NotBeNull();
        result.Amount.Should().Be(100m);
        result.Base.Should().Be("USD");
        result.Date.Should().Be(new DateOnly(2025, 1, 15));
        result.Rates.Should().HaveCount(2);
        result.Rates.Should().ContainKey("EUR").WhoseValue.Should().Be(92m);
        result.Rates.Should().ContainKey("GBP").WhoseValue.Should().Be(78m);
    }

    [Fact]
    public void ToCurrencyConversionResult_ExchangeRateWithEmptyRates_ReturnsEmptyRatesDictionary()
    {
        var exchangeRate = new ExchangeRate
        {
            Amount = new Amount(1m),
            Base = new Currency("USD"),
            Date = new ExchangeDate(new DateOnly(2025, 1, 15)),
            Rates = new Dictionary<Currency, Amount>()
        };

        var result = exchangeRate.ToCurrencyConversionResult();

        result.Rates.Should().BeEmpty();
    }
}
