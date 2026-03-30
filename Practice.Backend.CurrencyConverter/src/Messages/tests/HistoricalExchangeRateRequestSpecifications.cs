using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class HistoricalExchangeRateRequestSpecifications
{
    [Fact]
    public void BaseCurrency_WhenSet_RetainsValue()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.BaseCurrency.Should().Be("EUR");
    }

    [Fact]
    public void From_WhenSet_RetainsValue()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            From = new DateOnly(2024, 1, 1)
        };

        request.From.Should().Be(new DateOnly(2024, 1, 1));
    }

    [Fact]
    public void To_WhenSet_RetainsValue()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            To = new DateOnly(2024, 12, 31)
        };

        request.To.Should().Be(new DateOnly(2024, 12, 31));
    }

    [Fact]
    public void PageNumber_WhenSet_RetainsValue()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", PageNumber = 2 };

        request.PageNumber.Should().Be(2);
    }

    [Fact]
    public void DaysPerPage_WhenSet_RetainsValue()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", DaysPerPage = 7 };

        request.DaysPerPage.Should().Be(7);
    }

    [Fact]
    public void From_DefaultValue_IsNull()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.From.Should().BeNull();
    }

    [Fact]
    public void To_DefaultValue_IsNull()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.To.Should().BeNull();
    }

    [Fact]
    public void PageNumber_DefaultValue_IsNull()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.PageNumber.Should().BeNull();
    }

    [Fact]
    public void DaysPerPage_DefaultValue_IsNull()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.DaysPerPage.Should().BeNull();
    }

    [Fact]
    public void TwoRequests_WithSameValues_AreEqual()
    {
        var request1 = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", PageNumber = 1 };
        var request2 = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", PageNumber = 1 };

        request1.Should().Be(request2);
    }

    [Fact]
    public void ToString_ContainsBaseCurrency()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        request.ToString().Should().Contain("EUR");
    }
}
