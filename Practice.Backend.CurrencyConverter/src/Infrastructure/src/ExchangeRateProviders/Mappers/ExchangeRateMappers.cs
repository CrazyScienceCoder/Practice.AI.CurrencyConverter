using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Mappers;

public static class ExchangeRateMappers
{
    public static ExchangeRateSnapshot ToRateSnapshot(this LatestResponse response)
    {
        return new ExchangeRateSnapshot
        {
            Base = response.Base,
            Date = DateOnly.FromDateTime(response.Date),
            Rates = response.Rates.ToDictionary(r => Currency.Create(r.Key), r => Amount.Create((decimal)r.Value))
        };
    }

    public static HistoricalExchangeRateSnapshot ToRateSnapshot(this TimeSeriesResponse response)
    {
        return new HistoricalExchangeRateSnapshot
        {
            Amount = (decimal)response.Amount,
            Base = response.Base,
            StartDate = DateOnly.FromDateTime(response.StartDate),
            EndDate = DateOnly.FromDateTime(response.EndDate),
            Rates = response.Rates.ToDictionary(r => ExchangeDate.Create(DateOnly.ParseExact(r.Key, "yyyy-MM-dd")),
                r => r.Value.ToDictionary(i => Currency.Create(i.Key), i => Amount.Create((decimal)i.Value)))
        };
    }

    extension(ExchangeRateSnapshot snapshot)
    {
        public ExchangeRate ToExchangeRate(Amount amount)
        {
            return new ExchangeRate
            {
                Amount = amount,
                Base = snapshot.Base,
                Date = snapshot.Date,
                Rates = snapshot.Rates.ToDictionary(r => Currency.Create(r.Key), r => Amount.Create(r.Value * amount.Value))
            };
        }

        public ExchangeRate ToExchangeRate(Amount amount, Currency toCurrency)
        {
            return new ExchangeRate
            {
                Amount = amount,
                Base = snapshot.Base,
                Date = snapshot.Date,
                Rates = snapshot.Rates
                    .Where(r => r.Key.Value.Equals(toCurrency.Value, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(r => Currency.Create(r.Key), r => Amount.Create(r.Value * amount.Value))
            };
        }
    }

    public static HistoricalExchangeRate ToHistoricalExchangeRate(this HistoricalExchangeRateSnapshot snapshot)
    {
        return new HistoricalExchangeRate
        {
            Amount = snapshot.Amount,
            Base = snapshot.Base,
            StartDate = snapshot.StartDate,
            EndDate = snapshot.EndDate,
            Rates = snapshot.Rates
        };
    }
}
