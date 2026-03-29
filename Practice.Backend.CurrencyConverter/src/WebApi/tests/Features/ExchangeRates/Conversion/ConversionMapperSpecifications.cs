using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Conversion;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Conversion;

public sealed class ConversionMapperSpecifications
{
    private static ConversionRequest BuildValidRequest()
        => new()
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = null
        };

    [Fact]
    public void ToQuery_BaseCurrencyMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.BaseCurrency.Should().Be("USD");
    }

    [Fact]
    public void ToQuery_ToCurrencyMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.ToCurrency.Should().Be("EUR");
    }

    [Fact]
    public void ToQuery_AmountMappedCorrectly()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.Amount.Should().Be(100m);
    }

    [Fact]
    public void ToQuery_NullProvider_MapsToFrankfurterProvider()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void ToQuery_ExplicitFrankfurterProvider_MapsToFrankfurterProvider()
    {
        var request = BuildValidRequest() with { Provider = "Frankfurter" };

        var query = request.ToQuery();

        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void ToQuery_ReturnsGetCurrencyConversionQuery()
    {
        var request = BuildValidRequest();

        var query = request.ToQuery();

        query.Should().BeOfType<GetCurrencyConversionQuery>();
    }

    [Theory]
    [InlineData("USD", "EUR", 50.0)]
    [InlineData("GBP", "JPY", 1000.0)]
    public void ToQuery_AllFieldsMappedCorrectly(string baseCurrency, string toCurrency, double amount)
    {
        var request = new ConversionRequest
        {
            BaseCurrency = baseCurrency,
            ToCurrency = toCurrency,
            Amount = (decimal)amount,
            Provider = null
        };

        var query = request.ToQuery();

        query.BaseCurrency.Should().Be(baseCurrency);
        query.ToCurrency.Should().Be(toCurrency);
        query.Amount.Should().Be((decimal)amount);
        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }
}
