using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Exceptions;
using Practice.Backend.CurrencyConverter.Client.Internal;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Client;

internal sealed class CurrencyConverterClient(
    System.Net.Http.HttpClient httpClient,
    IOptions<CurrencyConverterClientOptions> options,
    ILogger<CurrencyConverterClient> logger)
    : ICurrencyConverterClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new DateOnlyJsonConverter() }
    };

    public Task<ExchangeRateResponse> GetLatestExchangeRatesAsync(
        LatestExchangeRatesRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildEndpoint(options.Value, "latest");
        var query = QueryBuilder.ForLatest(request);

        return ExecuteAsync<ExchangeRateResponse>(endpoint, query, cancellationToken);
    }

    public Task<HistoricalExchangeRateResponse> GetHistoricalExchangeRatesAsync(
        HistoricalExchangeRateRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildEndpoint(options.Value, "historical");
        var query = QueryBuilder.ForHistorical(request);

        return ExecuteAsync<HistoricalExchangeRateResponse>(endpoint, query, cancellationToken);
    }

    public Task<ExchangeRateResponse> GetConversionAsync(
        ConversionRequest request,
        CancellationToken cancellationToken = default)
    {
        var endpoint = BuildEndpoint(options.Value, "conversion");
        var query = QueryBuilder.ForConversion(request);

        return ExecuteAsync<ExchangeRateResponse>(endpoint, query, cancellationToken);
    }

    private async Task<T> ExecuteAsync<T>(
        string endpoint,
        IReadOnlyDictionary<string, string?> query,
        CancellationToken cancellationToken)
    {
        var requestUri = QueryHelpers.AddQueryString(endpoint, query);

        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        logger.LogInformation(
            "Currency Converter API request dispatched. Endpoint={Endpoint}",
            endpoint);

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw await CurrencyConverterApiException.FromResponseAsync(
                response,
                requestUri,
                cancellationToken);
        }

        var payload = await response.Content.ReadFromJsonAsync<T>(
            JsonOptions,
            cancellationToken);

        CurrencyConverterApiException.ThrowIfNull(payload, response.StatusCode, requestUri);

        return payload;
    }

    private static string BuildEndpoint(CurrencyConverterClientOptions opts, string segment)
        => $"api/v{opts.ApiVersion}/exchange-rate/{segment}";

    private sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.ParseExact(reader.GetString()!, Format);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format));

        public override DateOnly ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.ParseExact(reader.GetString()!, Format);

        public override void WriteAsPropertyName(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WritePropertyName(value.ToString(Format));
    }
}
