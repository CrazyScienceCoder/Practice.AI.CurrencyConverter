using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Constants;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

public interface IFrankfurterApiClient
{
    Task<LatestResponse> GetLatestAsync(string baseCurrency = Currencies.Euro
        , double amount = 1
        , string[]? symbols = null
        , CancellationToken cancellationToken = default);

    Task<HistoricalResponse> GetHistoricalAsync(DateOnly date
        , string baseCurrency = Currencies.Euro
        , string[]? symbols = null
        , CancellationToken cancellationToken = default);

    Task<TimeSeriesResponse> GetTimeSeriesAsync(DateOnly from
        , DateOnly to
        , string baseCurrency = Currencies.Euro
        , string[]? symbols = null
        , CancellationToken cancellationToken = default);
}
