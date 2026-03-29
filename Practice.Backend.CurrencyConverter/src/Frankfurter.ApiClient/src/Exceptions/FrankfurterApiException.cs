using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;

public sealed class FrankfurterApiException(string message, HttpStatusCode statusCode, string? responseContent = null)
    : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;

    public string? ResponseContent { get; } = responseContent;

    public static async Task<FrankfurterApiException> FromHttpResponseAsync(HttpResponseMessage response
        , string requestUri
        , CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return new FrankfurterApiException(
            $"Frankfurter API request failed. Status={response.StatusCode}, Uri={requestUri}"
            , response.StatusCode
            , content);
    }

    public static void ThrowIfNull<T>([NotNull] T payload, HttpStatusCode statusCode)
    {
        if (payload is null)
        {
            throw new FrankfurterApiException("Frankfurter API returned an empty payload.", statusCode);
        }
    }
}