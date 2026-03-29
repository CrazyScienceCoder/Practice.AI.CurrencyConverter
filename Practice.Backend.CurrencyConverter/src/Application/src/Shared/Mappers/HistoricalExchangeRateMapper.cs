using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Application.Shared.Extensions;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Application.Shared.Mappers;

public static class HistoricalExchangeRateMapper
{
    extension(HistoricalExchangeRate historicalExchangeRate)
    {
        public HistoricalExchangeRate FilterExcludedCurrencies()
        {
            return historicalExchangeRate with
            {
                Rates = historicalExchangeRate.Rates
                    .ToDictionary(r => r.Key, r => r.Value.Where(v => v.Key.IsSupportedCurrency())
                        .ToDictionary(d => d.Key, d => d.Value))
            };
        }

        public GetHistoricalExchangeRateQueryResult ToHistoricalExchangeRateResult(int pageNumber
            , int totalNumberOfPages
            , bool hasMore)
        {
            return new GetHistoricalExchangeRateQueryResult
            {
                Amount = historicalExchangeRate.Amount,
                Base = historicalExchangeRate.Base,
                EndDate = historicalExchangeRate.EndDate,
                StartDate = historicalExchangeRate.StartDate,
                Rates = historicalExchangeRate.Rates.ToDictionary(r => r.Key.Value,
                    r => r.Value.ToDictionary(rr => rr.Key.Value, rr => rr.Value.Value)),
                PageNumber = pageNumber,
                TotalNumberOfPages = totalNumberOfPages,
                HasMore = hasMore
            };
        }
    }
}
