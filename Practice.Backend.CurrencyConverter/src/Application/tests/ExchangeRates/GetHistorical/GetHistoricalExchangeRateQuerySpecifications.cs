using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.GetHistorical;

public sealed class GetHistoricalExchangeRateQuerySpecifications
{
    [Fact]
    public void GetCurrencies_ReturnsBaseCurrency()
    {
        var query = new GetHistoricalExchangeRateQuery
        {
            BaseCurrency = "USD",
            From = new DateOnly(2025, 1, 1),
            To = new DateOnly(2025, 1, 5),
            Provider = ExchangeRateProvider.Frankfurter
        };

        var currencies = query.GetCurrencies().ToList();

        currencies.Should().HaveCount(1);
        currencies[0].Should().Be(new Currency("USD"));
    }
}
