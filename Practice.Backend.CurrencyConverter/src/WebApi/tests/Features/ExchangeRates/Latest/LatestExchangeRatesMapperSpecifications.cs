using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Latest;

public sealed class LatestExchangeRatesMapperSpecifications
{
    [Fact]
    public void ToQuery_BaseCurrencyMappedCorrectly()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };

        var query = request.ToQuery();

        query.BaseCurrency.Should().Be("USD");
    }

    [Fact]
    public void ToQuery_NullProvider_MapsToFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };

        var query = request.ToQuery();

        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void ToQuery_ExplicitFrankfurterProvider_MapsToFrankfurterProvider()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "Frankfurter" };

        var query = request.ToQuery();

        query.Provider.Should().Be(ExchangeRateProvider.Frankfurter);
    }

    [Fact]
    public void ToQuery_ReturnsGetLatestExchangeRateQuery()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "GBP", Provider = null };

        var query = request.ToQuery();

        query.Should().BeOfType<GetLatestExchangeRateQuery>();
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    public void ToQuery_DifferentBaseCurrencies_MapsCorrectly(string currency)
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = currency, Provider = null };

        var query = request.ToQuery();

        query.BaseCurrency.Should().Be(currency);
    }
}
