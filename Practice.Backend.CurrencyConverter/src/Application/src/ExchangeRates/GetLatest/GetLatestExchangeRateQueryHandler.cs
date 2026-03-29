using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;

public sealed class GetLatestExchangeRateQueryHandler(
    IExchangeRateProviderFactory providerFactory,
    ILogger<GetLatestExchangeRateQueryHandler> logger)
    : HandlerBase<GetLatestExchangeRateQuery, GetLatestExchangeRateQueryResponse, GetLatestExchangeRateQueryResult>(logger)
{
    protected override async Task<GetLatestExchangeRateQueryResponse> ExecuteAsync(GetLatestExchangeRateQuery request
        , CancellationToken cancellationToken)
    {
        var provider = providerFactory.Create(request.Provider!);

        var result = await provider.GetLatestExchangeRateAsync(baseCurrency: request.BaseCurrency
            , cancellationToken: cancellationToken);

        return result.Match(
            onValue: exchangeRate => GetLatestExchangeRateQueryResponse.Success(
                data: exchangeRate.FilterExcludedCurrencies().ToLatestExchangeRateResult(),
                message: "Exchange rate was retrieved successfully"),
            onError: errors =>
            {
                var error = errors.First();
                if (error.Type is ErrorOr.ErrorType.NotFound)
                {
                    return GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound,
                        message: $"Exchange rate was not found, BaseCurrency: {request.BaseCurrency}");
                }

                return GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic,
                    message: $"{error.Description}, BaseCurrency: {request.BaseCurrency}");
            });
    }
}
