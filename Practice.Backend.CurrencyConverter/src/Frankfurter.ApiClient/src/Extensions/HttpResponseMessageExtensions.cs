using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task EnsureSuccessAsync(this HttpResponseMessage response
        , string requestUri
        , CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        throw await FrankfurterApiException.FromHttpResponseAsync(response
            , requestUri
            , cancellationToken);
    }
}