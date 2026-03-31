using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class HistoricalExchangeRateResponseSpecifications
{
    private static HistoricalExchangeRateResponse BuildResponse() => new(
        Amount: 1m,
        Base: "EUR",
        StartDate: new DateOnly(2024, 1, 1),
        EndDate: new DateOnly(2024, 1, 15),
        Rates: new Dictionary<DateOnly, IReadOnlyDictionary<string, decimal>>
        {
            [new DateOnly(2024, 1, 15)] = new Dictionary<string, decimal> { ["USD"] = 1.08m }
        },
        PageNumber: 1,
        HasMore: false,
        TotalNumberOfPages: 1);

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
    public void StartDate_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.StartDate.Should().Be(new DateOnly(2024, 1, 1));
    }

    [Fact]
    public void EndDate_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.EndDate.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public void Rates_ContainsExpectedDate()
    {
        var response = BuildResponse();

        response.Rates.Should().ContainKey(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public void Rates_ContainsExpectedCurrencyForDate()
    {
        var response = BuildResponse();

        response.Rates[new DateOnly(2024, 1, 15)].Should().ContainKey("USD");
    }

    [Fact]
    public void PageNumber_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.PageNumber.Should().Be(1);
    }

    [Fact]
    public void HasMore_WhenFalse_ReturnsFalse()
    {
        var response = BuildResponse();

        response.HasMore.Should().BeFalse();
    }

    [Fact]
    public void HasMore_WhenTrue_ReturnsTrue()
    {
        var response = new HistoricalExchangeRateResponse(
            Amount: 1m,
            Base: "EUR",
            StartDate: new DateOnly(2024, 1, 1),
            EndDate: new DateOnly(2024, 1, 15),
            Rates: new Dictionary<DateOnly, IReadOnlyDictionary<string, decimal>>(),
            PageNumber: 1,
            HasMore: true,
            TotalNumberOfPages: 3);

        response.HasMore.Should().BeTrue();
    }

    [Fact]
    public void TotalNumberOfPages_ReturnsCorrectValue()
    {
        var response = BuildResponse();

        response.TotalNumberOfPages.Should().Be(1);
    }

    [Fact]
    public void TwoResponses_WithSameValues_AreEqual()
    {
        var rates = new Dictionary<DateOnly, IReadOnlyDictionary<string, decimal>>
        {
            [new DateOnly(2024, 1, 15)] = new Dictionary<string, decimal> { ["USD"] = 1.08m }
        };
        var response1 = new HistoricalExchangeRateResponse(
            Amount: 1m, Base: "EUR",
            StartDate: new DateOnly(2024, 1, 1), EndDate: new DateOnly(2024, 1, 15),
            Rates: rates, PageNumber: 1, HasMore: false, TotalNumberOfPages: 1);
        var response2 = new HistoricalExchangeRateResponse(
            Amount: 1m, Base: "EUR",
            StartDate: new DateOnly(2024, 1, 1), EndDate: new DateOnly(2024, 1, 15),
            Rates: rates, PageNumber: 1, HasMore: false, TotalNumberOfPages: 1);

        response1.Should().Be(response2);
    }

    [Fact]
    public void TwoResponses_WithDifferentBase_AreNotEqual()
    {
        var response1 = BuildResponse();
        var response2 = new HistoricalExchangeRateResponse(
            Amount: 1m,
            Base: "USD",
            StartDate: new DateOnly(2024, 1, 1),
            EndDate: new DateOnly(2024, 1, 15),
            Rates: new Dictionary<DateOnly, IReadOnlyDictionary<string, decimal>>(),
            PageNumber: 1,
            HasMore: false,
            TotalNumberOfPages: 1);

        response1.Should().NotBe(response2);
    }

    [Fact]
    public void ToString_ContainsBase()
    {
        var response = BuildResponse();

        response.ToString().Should().Contain("EUR");
    }
}
