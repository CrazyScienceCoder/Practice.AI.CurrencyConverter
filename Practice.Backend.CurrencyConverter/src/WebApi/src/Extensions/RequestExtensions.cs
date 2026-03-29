using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.WebApi.Extensions;

public static class RequestExtensions
{
    public static ExchangeRateProvider BuildProvider(this RequestBase request)
        => request.Provider is null
            ? ExchangeRateProvider.Frankfurter
            : ExchangeRateProvider.FromName(request.Provider, ignoreCase: true);
}
