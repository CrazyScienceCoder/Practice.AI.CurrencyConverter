using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Mappers;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Mappers;

public sealed class ExchangeRateMappersSpecifications
{
    [Fact]
    public void ToRateSnapshot_LatestResponse_MapsBaseCorrectly()
    {
        var response = new LatestResponse
        {
            Amount = 1,
            Base = "EUR",
            Date = new DateTime(2024, 1, 15),
            Rates = new() { { "USD", 1.08 } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Base.Value.Should().Be("EUR");
    }

    [Fact]
    public void ToRateSnapshot_LatestResponse_MapsDateCorrectly()
    {
        var response = new LatestResponse
        {
            Amount = 1,
            Base = "EUR",
            Date = new DateTime(2024, 1, 15),
            Rates = new() { { "USD", 1.08 } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Date.Value.Should().Be(DateOnly.FromDateTime(response.Date));
    }

    [Fact]
    public void ToRateSnapshot_LatestResponse_MapsRatesCorrectly()
    {
        var response = new LatestResponse
        {
            Amount = 1,
            Base = "EUR",
            Date = new DateTime(2024, 1, 15),
            Rates = new() { { "USD", 1.08 }, { "GBP", 0.86 } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Rates.Should().HaveCount(2);
        snapshot.Rates.Should().ContainKey(Currency.Create("USD"));
        snapshot.Rates[Currency.Create("USD")].Value.Should().Be((decimal)1.08);
    }

    [Fact]
    public void ToRateSnapshot_LatestResponseWithEmptyRates_MapsToEmptyRatesDictionary()
    {
        var response = new LatestResponse
        {
            Amount = 1,
            Base = "EUR",
            Date = new DateTime(2024, 1, 15),
            Rates = new()
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Rates.Should().BeEmpty();
    }

    [Fact]
    public void ToRateSnapshot_TimeSeriesResponse_MapsBaseCorrectly()
    {
        var response = new TimeSeriesResponse
        {
            Amount = 1,
            Base = "EUR",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 15),
            Rates = new() { { "2024-01-01", new() { { "USD", 1.08 } } } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Base.Value.Should().Be("EUR");
    }

    [Fact]
    public void ToRateSnapshot_TimeSeriesResponse_MapsAmountCorrectly()
    {
        var response = new TimeSeriesResponse
        {
            Amount = 5.0,
            Base = "EUR",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 15),
            Rates = new() { { "2024-01-01", new() { { "USD", 1.08 } } } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Amount.Value.Should().Be((decimal)response.Amount);
    }

    [Fact]
    public void ToRateSnapshot_TimeSeriesResponse_MapsStartAndEndDatesCorrectly()
    {
        var response = new TimeSeriesResponse
        {
            Amount = 1,
            Base = "EUR",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 15),
            Rates = new() { { "2024-01-01", new() { { "USD", 1.08 } } } }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.StartDate.Value.Should().Be(DateOnly.FromDateTime(response.StartDate));
        snapshot.EndDate.Value.Should().Be(DateOnly.FromDateTime(response.EndDate));
    }

    [Fact]
    public void ToRateSnapshot_TimeSeriesResponse_MapsRatesByDateCorrectly()
    {
        var response = new TimeSeriesResponse
        {
            Amount = 1,
            Base = "EUR",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 2),
            Rates = new()
            {
                { "2024-01-01", new() { { "USD", 1.08 } } },
                { "2024-01-02", new() { { "USD", 1.09 } } }
            }
        };

        var snapshot = response.ToRateSnapshot();

        snapshot.Rates.Should().HaveCount(2);
    }

    [Fact]
    public void ToRateSnapshot_TimeSeriesResponse_MapsNestedCurrencyRatesCorrectly()
    {
        var response = new TimeSeriesResponse
        {
            Amount = 1,
            Base = "EUR",
            StartDate = new DateTime(2024, 1, 1),
            EndDate = new DateTime(2024, 1, 1),
            Rates = new() { { "2024-01-01", new() { { "USD", 1.08 }, { "GBP", 0.86 } } } }
        };

        var snapshot = response.ToRateSnapshot();

        var dayRates = snapshot.Rates[ExchangeDate.Create(new DateOnly(2024, 1, 1))];
        dayRates.Should().HaveCount(2);
        dayRates.Should().ContainKey(Currency.Create("USD"));
        dayRates[Currency.Create("USD")].Value.Should().Be((decimal)1.08);
    }

    [Fact]
    public void ToExchangeRate_WithAmount_MapsBaseCorrectly()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(100m);

        var rate = snapshot.ToExchangeRate(amount);

        rate.Base.Value.Should().Be("EUR");
    }

    [Fact]
    public void ToExchangeRate_WithAmount_MapsDateCorrectly()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(1m);

        var rate = snapshot.ToExchangeRate(amount);

        rate.Date.Value.Should().Be(snapshot.Date.Value);
    }

    [Fact]
    public void ToExchangeRate_WithAmount_MultipliesRatesByAmount()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(100m);

        var rate = snapshot.ToExchangeRate(amount);

        rate.Rates[Currency.Create("USD")].Value.Should().Be(108m);
    }

    [Fact]
    public void ToExchangeRate_WithAmount_SetsAmountOnResult()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(50m);

        var rate = snapshot.ToExchangeRate(amount);

        rate.Amount.Value.Should().Be(50m);
    }

    [Fact]
    public void ToExchangeRate_WithAmountAndTargetCurrency_FiltersToOnlyTargetCurrency()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
            {
                { Currency.Create("USD"), Amount.Create(1.08m) },
                { Currency.Create("GBP"), Amount.Create(0.86m) }
            }
        };
        var amount = Amount.Create(100m);
        var toCurrency = Currency.Create("USD");

        var rate = snapshot.ToExchangeRate(amount, toCurrency);

        rate.Rates.Should().HaveCount(1);
        rate.Rates.Should().ContainKey(Currency.Create("USD"));
        rate.Rates.Should().NotContainKey(Currency.Create("GBP"));
    }

    [Fact]
    public void ToExchangeRate_WithAmountAndTargetCurrency_MultipliesRateByAmount()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(100m);
        var toCurrency = Currency.Create("USD");

        var rate = snapshot.ToExchangeRate(amount, toCurrency);

        rate.Rates[Currency.Create("USD")].Value.Should().Be(108m);
    }

    [Fact]
    public void ToExchangeRate_WithAmountAndNonExistentTargetCurrency_ReturnsEmptyRates()
    {
        var snapshot = new ExchangeRateSnapshot
        {
            Base = Currency.Create("EUR"),
            Date = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new() { { Currency.Create("USD"), Amount.Create(1.08m) } }
        };
        var amount = Amount.Create(100m);
        var toCurrency = Currency.Create("JPY");

        var rate = snapshot.ToExchangeRate(amount, toCurrency);

        rate.Rates.Should().BeEmpty();
    }

    [Fact]
    public void ToHistoricalExchangeRate_Always_MapsAmountCorrectly()
    {
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(2m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
        };

        var result = snapshot.ToHistoricalExchangeRate();

        result.Amount.Value.Should().Be(2m);
    }

    [Fact]
    public void ToHistoricalExchangeRate_Always_MapsBaseCorrectly()
    {
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("USD"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
        };

        var result = snapshot.ToHistoricalExchangeRate();

        result.Base.Value.Should().Be("USD");
    }

    [Fact]
    public void ToHistoricalExchangeRate_Always_MapsDateRangeCorrectly()
    {
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
        };

        var result = snapshot.ToHistoricalExchangeRate();

        result.StartDate.Value.Should().Be(snapshot.StartDate.Value);
        result.EndDate.Value.Should().Be(snapshot.EndDate.Value);
    }

    [Fact]
    public void ToHistoricalExchangeRate_Always_MapsRatesCorrectly()
    {
        var dateKey = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
            {
                { dateKey, new() { { Currency.Create("USD"), Amount.Create(1.08m) } } }
            }
        };

        var result = snapshot.ToHistoricalExchangeRate();

        result.Rates.Should().HaveCount(1);
        result.Rates.Should().ContainKey(dateKey);
    }

    [Fact]
    public void ToHistoricalExchangeRate_WithEmptyRates_MapsToEmptyRates()
    {
        var snapshot = new HistoricalExchangeRateSnapshot
        {
            Amount = Amount.Create(1m),
            Base = Currency.Create("EUR"),
            StartDate = ExchangeDate.Create(new DateOnly(2024, 1, 1)),
            EndDate = ExchangeDate.Create(new DateOnly(2024, 1, 15)),
            Rates = new()
        };

        var result = snapshot.ToHistoricalExchangeRate();

        result.Rates.Should().BeEmpty();
    }
}
