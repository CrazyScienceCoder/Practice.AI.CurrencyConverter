using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.CurrencyConversion;

public sealed class GetCurrencyConversionQueryHandler(
    IExchangeRateProviderFactory providerFactory,
    ILogger<GetCurrencyConversionQueryHandler> logger)
    : HandlerBase<GetCurrencyConversionQuery, GetCurrencyConversionQueryResponse, GetCurrencyConversionQueryResult>(logger)
{
    protected override async Task<GetCurrencyConversionQueryResponse> ExecuteAsync(GetCurrencyConversionQuery request,
        CancellationToken cancellationToken)
    {
        var provider = providerFactory.Create(request.Provider!);

        var result = await provider.ConvertAsync(baseCurrency: request.BaseCurrency
            , toCurrency: request.ToCurrency
            , amount: request.Amount
            , cancellationToken: cancellationToken);

        return result.Match(
            onValue: exchangeRate => GetCurrencyConversionQueryResponse.Success(
                data: exchangeRate.ToCurrencyConversionResult(),
                message: "Exchange rate was retrieved successfully"),
            onError: errors =>
            {
                var error = errors.First();
                if (error.Type is ErrorOr.ErrorType.NotFound)
                {
                    return GetCurrencyConversionQueryResponse.Failure(errorType: ErrorType.NotFound,
                        message: $"Exchange rate was not found, BaseCurrency: {request.BaseCurrency}");
                }

                return GetCurrencyConversionQueryResponse.Failure(errorType: ErrorType.Generic,
                    message: $"{error.Description}, BaseCurrency: {request.BaseCurrency}");
            });
    }
}
