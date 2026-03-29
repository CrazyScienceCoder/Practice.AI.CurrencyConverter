using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Practice.Backend.CurrencyConverter.Client.Exceptions;

public sealed class CurrencyConverterApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string? ResponseContent { get; }

    public string? RequestUri { get; }

    private CurrencyConverterApiException(
        string message,
        HttpStatusCode statusCode,
        string? requestUri,
        string? responseContent)
        : base(message)
    {
        StatusCode = statusCode;
        RequestUri = requestUri;
        ResponseContent = responseContent;
    }

    internal static async Task<CurrencyConverterApiException> FromResponseAsync(
        HttpResponseMessage response,
        string requestUri,
        CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return new CurrencyConverterApiException(
            $"Currency Converter API request failed. Status={response.StatusCode}, Uri={requestUri}",
            response.StatusCode,
            requestUri,
            content);
    }

    internal static void ThrowIfNull<T>(
        [NotNull] T? payload,
        HttpStatusCode statusCode,
        string requestUri)
    {
        if (payload is null)
        {
            throw new CurrencyConverterApiException(
                $"Currency Converter API returned an empty or undeserializable response. Uri={requestUri}",
                statusCode,
                requestUri,
                responseContent: null);
        }
    }
}
