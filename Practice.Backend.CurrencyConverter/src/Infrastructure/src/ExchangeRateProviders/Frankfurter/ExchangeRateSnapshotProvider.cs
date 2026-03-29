using System.Net;
using ErrorOr;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Mappers;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

public sealed class ExchangeRateSnapshotProvider(
    IFrankfurterApiClient frankfurterClient,
    ILogger<ExchangeRateSnapshotProvider> logger) : IExchangeRateSnapshotProvider
{
    public ExchangeRateProvider Provider => ExchangeRateProvider.Frankfurter;

    public async Task<ErrorOr<ExchangeRateSnapshot>> GetLatestAsync(Currency baseCurrency
        , CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            var response = await frankfurterClient.GetLatestAsync(baseCurrency: baseCurrency
                , cancellationToken: cancellationToken);

            return response.ToRateSnapshot();
        });
    }

    public async Task<ErrorOr<HistoricalExchangeRateSnapshot>> GetTimeSeriesAsync(Currency baseCurrency
        , ExchangeDate from
        , ExchangeDate to
        , CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            var response = await frankfurterClient.GetTimeSeriesAsync(from: from
                , to: to
                , baseCurrency: baseCurrency
                , cancellationToken: cancellationToken);

            return response.ToRateSnapshot();
        });
    }

    private async Task<ErrorOr<TResult>> ExecuteAsync<TResult>(Func<Task<TResult>> func)
    {
        try
        {
            return await func();
        }
        catch (FrankfurterApiException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return Error.NotFound();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error while calling the Frankfurter API.");
            return Error.Unexpected(description: exception.Message);
        }
    }
}
