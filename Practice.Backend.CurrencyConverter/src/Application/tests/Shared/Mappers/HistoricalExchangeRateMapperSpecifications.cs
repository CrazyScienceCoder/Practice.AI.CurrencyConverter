using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Application.Tests.Shared.Mappers;

public sealed class HistoricalExchangeRateMapperSpecifications
{
    [Fact]
    public void FilterExcludedCurrencies_WithNoForbiddenCurrencies_ReturnsAllRates()
    {
        var historicalRate = BuildHistoricalExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) },
            { new Currency("GBP"), new Amount(0.78m) }
        });

        var result = historicalRate.FilterExcludedCurrencies();

        foreach (var dailyRates in result.Rates.Values)
        {
            dailyRates.Should().ContainKey(new Currency("EUR"));
            dailyRates.Should().ContainKey(new Currency("GBP"));
        }
    }

    [Fact]
    public void FilterExcludedCurrencies_WithForbiddenCurrencies_RemovesForbiddenFromEachDay()
    {
        var historicalRate = BuildHistoricalExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("EUR"), new Amount(0.92m) },
            { new Currency("MXN"), new Amount(17.5m) },
            { new Currency("PLN"), new Amount(4.1m) }
        });

        var result = historicalRate.FilterExcludedCurrencies();

        foreach (var dailyRates in result.Rates.Values)
        {
            dailyRates.Should().ContainKey(new Currency("EUR"));
            dailyRates.Should().NotContainKey(new Currency("MXN"));
            dailyRates.Should().NotContainKey(new Currency("PLN"));
        }
    }

    [Fact]
    public void FilterExcludedCurrencies_WithAllForbiddenCurrencies_ReturnsEmptyDailyRates()
    {
        var historicalRate = BuildHistoricalExchangeRate(new Dictionary<Currency, Amount>
        {
            { new Currency("MXN"), new Amount(17.5m) },
            { new Currency("PLN"), new Amount(4.1m) }
        });

        var result = historicalRate.FilterExcludedCurrencies();

        foreach (var dailyRates in result.Rates.Values)
        {
            dailyRates.Should().BeEmpty();
        }
    }

    [Fact]
    public void FilterExcludedCurrencies_PreservesOriginalHistoricalRateProperties()
    {
        var startDate = new DateOnly(2025, 1, 13);
        var endDate = new DateOnly(2025, 1, 17);
        var historicalRate = BuildHistoricalExchangeRate(
            new Dictionary<Currency, Amount> { { new Currency("EUR"), new Amount(0.92m) } },
            startDate, endDate);

        var result = historicalRate.FilterExcludedCurrencies();

        result.Amount.Value.Should().Be(1m);
        result.Base.Value.Should().Be("USD");
        result.StartDate.Value.Should().Be(startDate);
        result.EndDate.Value.Should().Be(endDate);
    }

    [Fact]
    public void ToHistoricalExchangeRateResult_ValidData_MapsAllPropertiesCorrectly()
    {
        var startDate = new DateOnly(2025, 1, 13);
        var endDate = new DateOnly(2025, 1, 17);
        var historicalRate = BuildHistoricalExchangeRate(
            new Dictionary<Currency, Amount>
            {
                { new Currency("EUR"), new Amount(0.92m) }
            },
            startDate, endDate);

        const int pageNumber = 1;
        const int totalPages = 3;
        const bool hasMore = true;

        var result = historicalRate.ToHistoricalExchangeRateResult(pageNumber, totalPages, hasMore);

        result.Should().NotBeNull();
        result.Amount.Should().Be(1m);
        result.Base.Should().Be("USD");
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(endDate);
        result.PageNumber.Should().Be(pageNumber);
        result.TotalNumberOfPages.Should().Be(totalPages);
        result.HasMore.Should().Be(hasMore);
        result.Rates.Should().NotBeEmpty();
        result.Rates.Values.Should().AllSatisfy(
            dailyRates => dailyRates.Should().ContainKey("EUR").WhoseValue.Should().Be(0.92m));
    }

    [Fact]
    public void ToHistoricalExchangeRateResult_LastPage_SetsHasMoreToFalse()
    {
        var historicalRate = BuildHistoricalExchangeRate(
            new Dictionary<Currency, Amount> { { new Currency("EUR"), new Amount(0.92m) } });

        var result = historicalRate.ToHistoricalExchangeRateResult(pageNumber: 2, totalNumberOfPages: 2, hasMore: false);

        result.HasMore.Should().BeFalse();
        result.PageNumber.Should().Be(2);
        result.TotalNumberOfPages.Should().Be(2);
    }

    [Fact]
    public void ToHistoricalExchangeRateResult_RatesDictionaryContainsExpectedCurrencyStringKeys()
    {
        var startDate = new DateOnly(2025, 1, 13);
        var endDate = new DateOnly(2025, 1, 15);
        var historicalRate = BuildHistoricalExchangeRate(
            new Dictionary<Currency, Amount> { { new Currency("EUR"), new Amount(0.92m) } },
            startDate, endDate);

        var result = historicalRate.ToHistoricalExchangeRateResult(1, 1, false);

        result.Rates.Should().NotBeEmpty();
        result.Rates.Values.Should().AllSatisfy(v => v.Should().ContainKey("EUR"));
    }

    private static HistoricalExchangeRate BuildHistoricalExchangeRate(
        Dictionary<Currency, Amount> dailyRates,
        DateOnly? startDate = null,
        DateOnly? endDate = null)
    {
        var from = startDate ?? new DateOnly(2025, 1, 13);
        var to = endDate ?? new DateOnly(2025, 1, 17);
        var rates = new Dictionary<ExchangeDate, Dictionary<Currency, Amount>>();

        for (var date = from; date <= to; date = date.AddDays(1))
        {
            if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            rates[new ExchangeDate(date)] = new Dictionary<Currency, Amount>(dailyRates);
        }

        return new HistoricalExchangeRate
        {
            Amount = new Amount(1m),
            Base = new Currency("USD"),
            StartDate = new ExchangeDate(from),
            EndDate = new ExchangeDate(to),
            Rates = rates
        };
    }
}
