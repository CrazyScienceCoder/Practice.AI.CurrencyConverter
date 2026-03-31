using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class LatestExchangeRatesRequestSpecifications
{
    [Fact]
    public void BaseCurrency_WhenSet_RetainsValue()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR" };

        request.BaseCurrency.Should().Be("EUR");
    }

    [Fact]
    public void Provider_WhenSet_RetainsValue()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "frankfurter" };

        request.Provider.Should().Be("frankfurter");
    }

    [Fact]
    public void TwoRequests_WithSameValues_AreEqual()
    {
        var request1 = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "frankfurter" };
        var request2 = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "frankfurter" };

        request1.Should().Be(request2);
    }

    [Fact]
    public void TwoRequests_WithDifferentBaseCurrencies_AreNotEqual()
    {
        var request1 = new LatestExchangeRatesRequest { BaseCurrency = "EUR" };
        var request2 = new LatestExchangeRatesRequest { BaseCurrency = "USD" };

        request1.Should().NotBe(request2);
    }

    [Fact]
    public void ToString_ContainsBaseCurrency()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR" };

        request.ToString().Should().Contain("EUR");
    }
}
