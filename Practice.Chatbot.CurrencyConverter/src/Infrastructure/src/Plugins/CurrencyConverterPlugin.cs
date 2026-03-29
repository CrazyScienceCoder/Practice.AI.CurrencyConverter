using System.ComponentModel;
using System.Net;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Client;
using Practice.Backend.CurrencyConverter.Client.Exceptions;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

public sealed class CurrencyConverterPlugin(
    ICurrencyConverterClient currencyConverterClient,
    ILogger<CurrencyConverterPlugin> logger)
{
    [Description("Gets the latest exchange rates for a given base currency. Returns rates for all supported currencies.")]
    public async Task<PluginResult<ExchangeRateResponse>> GetLatestExchangeRatesAsync(
        [Description("ISO 4217 base currency code (e.g. USD, EUR, GBP)")] string baseCurrency,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching latest rates for base currency {BaseCurrency}", baseCurrency);

        try
        {
            var response = await currencyConverterClient.GetLatestExchangeRatesAsync(
                new LatestExchangeRatesRequest { BaseCurrency = baseCurrency, Provider = "frankfurter" },
                cancellationToken);

            return PluginResult<ExchangeRateResponse>.Success(response);
        }
        catch (CurrencyConverterApiException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            logger.LogError(ex, "Unauthorized access when fetching latest rates for {BaseCurrency}", baseCurrency);
            return PluginResult<ExchangeRateResponse>.Failure("You are not authorized to perform this action.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch latest rates for {BaseCurrency}", baseCurrency);
            return PluginResult<ExchangeRateResponse>.Failure($"Could not fetch rates for {baseCurrency}: {ex.Message}");
        }
    }

    [Description("Converts an amount from one currency to another using current exchange rates.")]
    public async Task<PluginResult<ExchangeRateResponse>> ConvertCurrencyAsync(
        [Description("ISO 4217 source currency code (e.g. USD)")] string fromCurrency,
        [Description("ISO 4217 target currency code (e.g. EUR)")] string toCurrency,
        [Description("The amount to convert (positive number)")] decimal amount,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Converting {Amount} {From} to {To}", amount, fromCurrency, toCurrency);

        try
        {
            var response = await currencyConverterClient.GetConversionAsync(
                new ConversionRequest
                {
                    BaseCurrency = fromCurrency,
                    ToCurrency = toCurrency,
                    Amount = amount,
                    Provider = "frankfurter"
                },
                cancellationToken);

            return PluginResult<ExchangeRateResponse>.Success(response);
        }
        catch (CurrencyConverterApiException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            logger.LogError(ex, "Unauthorized access when converting {Amount} {From} to {To}", amount, fromCurrency, toCurrency);
            return PluginResult<ExchangeRateResponse>.Failure("You are not authorized to perform this action.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert {Amount} {From} to {To}", amount, fromCurrency, toCurrency);
            return PluginResult<ExchangeRateResponse>.Failure($"Could not convert {amount} {fromCurrency} to {toCurrency}: {ex.Message}");
        }
    }

    [Description("Gets historical exchange rates for a base currency over a date range.")]
    public async Task<PluginResult<HistoricalExchangeRateResponse>> GetHistoricalExchangeRatesAsync(
        [Description("ISO 4217 base currency code (e.g. USD)")] string baseCurrency,
        [Description("Start date in YYYY-MM-DD format")] string fromDate,
        [Description("End date in YYYY-MM-DD format")] string toDate,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching historical rates for {BaseCurrency} from {From} to {To}",
            baseCurrency, fromDate, toDate);

        try
        {
            var response = await currencyConverterClient.GetHistoricalExchangeRatesAsync(
                new HistoricalExchangeRateRequest
                {
                    BaseCurrency = baseCurrency,
                    From = DateOnly.Parse(fromDate),
                    To = DateOnly.Parse(toDate),
                    Provider = "frankfurter"
                },
                cancellationToken);

            return PluginResult<HistoricalExchangeRateResponse>.Success(response);
        }
        catch (CurrencyConverterApiException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            logger.LogError(ex, "Unauthorized access when fetching historical rates for {BaseCurrency}", baseCurrency);
            return PluginResult<HistoricalExchangeRateResponse>.Failure("You are not authorized to perform this action.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch historical rates for {BaseCurrency}", baseCurrency);
            return PluginResult<HistoricalExchangeRateResponse>.Failure($"Could not fetch historical rates: {ex.Message}");
        }
    }
}
