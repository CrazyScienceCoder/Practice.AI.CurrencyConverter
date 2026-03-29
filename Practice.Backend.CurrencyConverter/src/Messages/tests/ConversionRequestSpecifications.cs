using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class ConversionRequestSpecifications
{
    [Fact]
    public void BaseCurrency_WhenSet_RetainsValue()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        request.BaseCurrency.Should().Be("EUR");
    }

    [Fact]
    public void ToCurrency_WhenSet_RetainsValue()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        request.ToCurrency.Should().Be("USD");
    }

    [Fact]
    public void Amount_WhenSet_RetainsValue()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 99.99m };

        request.Amount.Should().Be(99.99m);
    }

    [Fact]
    public void Provider_WhenSet_RetainsValue()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100, Provider = "frankfurter" };

        request.Provider.Should().Be("frankfurter");
    }

    [Fact]
    public void TwoRequests_WithSameValues_AreEqual()
    {
        var request1 = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };
        var request2 = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        request1.Should().Be(request2);
    }

    [Fact]
    public void TwoRequests_WithDifferentAmounts_AreNotEqual()
    {
        var request1 = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };
        var request2 = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 200 };

        request1.Should().NotBe(request2);
    }

    [Fact]
    public void ToString_ContainsBaseCurrency()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        request.ToString().Should().Contain("EUR");
    }
}
