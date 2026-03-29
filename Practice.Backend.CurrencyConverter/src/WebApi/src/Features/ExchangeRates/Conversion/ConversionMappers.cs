using Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.WebApi.Extensions;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Conversion;

public static class ConversionMappers
{
    public static GetCurrencyConversionQuery ToQuery(this ConversionRequest request)
    {
        return new GetCurrencyConversionQuery
        {
            Amount = request.Amount,
            BaseCurrency = request.BaseCurrency,
            Provider = request.BuildProvider(),
            ToCurrency = request.ToCurrency
        };
    }
}