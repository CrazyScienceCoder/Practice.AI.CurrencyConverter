using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Application.Shared.Mappers;

public static class CurrencyConversionMapper
{
    public static GetCurrencyConversionQueryResult ToCurrencyConversionResult(this ExchangeRate exchangeRate)
    {
        return new GetCurrencyConversionQueryResult
        {
            Amount = exchangeRate.Amount,
            Base = exchangeRate.Base,
            Date = exchangeRate.Date,
            Rates = exchangeRate.Rates.ToDictionary(r => r.Key.Value, r => r.Value.Value)
        };
    }
}
