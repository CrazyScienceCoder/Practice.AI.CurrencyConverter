using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

public static class LatestExchangeRatesMappers
{
    public static GetLatestExchangeRateQuery ToQuery(this LatestExchangeRatesRequest request)
    {
        return new GetLatestExchangeRateQuery
        {
            BaseCurrency = request.BaseCurrency,
            Provider = request.BuildProvider()
        };
    }
}