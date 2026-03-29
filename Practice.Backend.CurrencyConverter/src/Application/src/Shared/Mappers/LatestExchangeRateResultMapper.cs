using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared.Extensions;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Application.Shared.Mappers;

public static class ExchangeRateMapper
{
    extension(ExchangeRate exchangeRate)
    {
        public ExchangeRate FilterExcludedCurrencies()
        {
            return exchangeRate with
            {
                Rates = exchangeRate.Rates
                    .Where(r => r.Key.IsSupportedCurrency())
                    .ToDictionary(r => r.Key, r => r.Value)
            };
        }

        public GetLatestExchangeRateQueryResult ToLatestExchangeRateResult()
        {
            return new GetLatestExchangeRateQueryResult
            {
                Amount = exchangeRate.Amount,
                Base = exchangeRate.Base,
                Date = exchangeRate.Date,
                Rates = exchangeRate.Rates
                    .ToDictionary(r => r.Key.Value, r => r.Value.Value)
            };
        }
    }
}