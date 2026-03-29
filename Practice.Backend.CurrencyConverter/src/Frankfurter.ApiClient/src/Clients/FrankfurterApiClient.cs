using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Constants;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

public sealed class FrankfurterApiClient(HttpClient httpClient, ILogger<FrankfurterApiClient> logger)
    : IFrankfurterApiClient
{
    public Task<LatestResponse> GetLatestAsync(string baseCurrency = Currencies.Euro
        , double amount = 1
        , string[]? symbols = null
        , CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<LatestResponse>(endpoint: FrankfurterEndpoints.Latest
            , query: QueryBuilder
                .Create(baseCurrency)
                .WithAmount(amount)
                .WithSymbols(symbols)
                .Build()
            , cancellationToken: cancellationToken);
    }

    public Task<HistoricalResponse> GetHistoricalAsync(DateOnly date
        , string baseCurrency = Currencies.Euro
        , string[]? symbols = null
        , CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<HistoricalResponse>(endpoint: FrankfurterEndpoints.ForDate(date)
            , query: QueryBuilder
                .Create(baseCurrency)
                .WithSymbols(symbols)
                .Build()
            , cancellationToken: cancellationToken);
    }

    public Task<TimeSeriesResponse> GetTimeSeriesAsync(DateOnly from
        , DateOnly to
        , string baseCurrency = Currencies.Euro
        , string[]? symbols = null
        , CancellationToken cancellationToken = default)
    {
        return ExecuteAsync<TimeSeriesResponse>(endpoint: FrankfurterEndpoints.ForRange(from, to)
            , query: QueryBuilder
                .Create(baseCurrency)
                .WithSymbols(symbols)
                .Build()
            , cancellationToken: cancellationToken);
    }

    private async Task<T> ExecuteAsync<T>(string endpoint
        , IReadOnlyDictionary<string, string?> query
        , CancellationToken cancellationToken)
    {
        var requestUri = QueryHelpers.AddQueryString(endpoint, query);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        logger.LogInformation("Frankfurter API request dispatched. Endpoint={Endpoint}", endpoint);

        using var response = await httpClient.SendAsync(request
            , HttpCompletionOption.ResponseHeadersRead
            , cancellationToken);

        await response.EnsureSuccessAsync(requestUri, cancellationToken);

        var payload = await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

        FrankfurterApiException.ThrowIfNull(payload, response.StatusCode);

        return payload;
    }
}