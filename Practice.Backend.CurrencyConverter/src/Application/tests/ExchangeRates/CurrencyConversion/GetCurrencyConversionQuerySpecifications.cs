using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.CurrencyConversion;

public sealed class GetCurrencyConversionQuerySpecifications
{
    [Fact]
    public void GetCurrencies_ReturnsBothBaseCurrencyAndToCurrency()
    {
        var query = new GetCurrencyConversionQuery
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = ExchangeRateProvider.Frankfurter
        };

        var currencies = query.GetCurrencies().ToList();

        currencies.Should().HaveCount(2);
        currencies[0].Should().Be(new Currency("USD"));
        currencies[1].Should().Be(new Currency("EUR"));
    }
}
