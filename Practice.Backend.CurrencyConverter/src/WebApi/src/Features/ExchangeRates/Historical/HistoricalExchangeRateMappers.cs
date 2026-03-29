using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

public static class HistoricalExchangeRateMappers
{
    public static GetHistoricalExchangeRateQuery ToQuery(this HistoricalExchangeRateRequest request)
    {
        return new GetHistoricalExchangeRateQuery
        {
            BaseCurrency = request.BaseCurrency,
            Provider = request.BuildProvider(),
            From = request.From!.Value,
            To = request.To!.Value,
            DaysPerPage = request.DaysPerPage,
            PageNumber = request.PageNumber
        };
    }
}