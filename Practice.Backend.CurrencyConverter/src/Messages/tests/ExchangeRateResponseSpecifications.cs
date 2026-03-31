using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class ExchangeRateResponseSpecifications
{
    private static ExchangeRateResponse BuildResponse() => new(
        Amount: 1m,
        Base: "EUR",
        Date: new DateOnly(2024, 1, 15),
        Rates: new Dictionary<string, decimal> { ["USD"] = 1.08m, ["GBP"] = 0.86m });

    [Fact]
    public void Amount_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.Amount.Should().Be(1m);
    }

    [Fact]
    public void Base_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.Base.Should().Be("EUR");
    }

    [Fact]
    public void Date_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.Date.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public void Rates_ContainsExpectedCurrencies()
    {
        var response = BuildResponse();

        response.Rates.Should().ContainKey("USD");
        response.Rates.Should().ContainKey("GBP");
    }

    [Fact]
    public void Rates_ReturnsCorrectValues()
    {
        var response = BuildResponse();

        response.Rates["USD"].Should().Be(1.08m);
        response.Rates["GBP"].Should().Be(0.86m);
    }

    [Fact]
    public void TwoResponses_WithSameValues_AreEqual()
    {
        var rates = new Dictionary<string, decimal> { ["USD"] = 1.08m, ["GBP"] = 0.86m };
        var response1 = new ExchangeRateResponse(1m, "EUR", new DateOnly(2024, 1, 15), rates);
        var response2 = new ExchangeRateResponse(1m, "EUR", new DateOnly(2024, 1, 15), rates);

        response1.Should().Be(response2);
    }

    [Fact]
    public void TwoResponses_WithDifferentBase_AreNotEqual()
    {
        var response1 = BuildResponse();
        var response2 = new ExchangeRateResponse(
            Amount: 1m,
            Base: "USD",
            Date: new DateOnly(2024, 1, 15),
            Rates: new Dictionary<string, decimal>());

        response1.Should().NotBe(response2);
    }

    [Fact]
    public void ToString_ContainsBase()
    {
        var response = BuildResponse();

        response.ToString().Should().Contain("EUR");
    }
}
