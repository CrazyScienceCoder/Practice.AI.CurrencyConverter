using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Client;

public interface ICurrencyConverterClient
{
    Task<ExchangeRateResponse> GetLatestExchangeRatesAsync(
        LatestExchangeRatesRequest request,
        CancellationToken cancellationToken = default);

    Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync(
        HistoricalExchangeRateRequest request,
        CancellationToken cancellationToken = default);

    Task<ExchangeRateResponse> GetConversionAsync(
        ConversionRequest request,
        CancellationToken cancellationToken = default);
}
