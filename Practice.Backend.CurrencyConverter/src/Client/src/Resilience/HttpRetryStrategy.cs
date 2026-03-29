using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Practice.Backend.CurrencyConverter.Client.Configuration;

namespace Practice.Backend.CurrencyConverter.Client.Resilience;

internal static class HttpRetryStrategy
{
    internal static HttpRetryStrategyOptions Create(
        ILogger logger,
        CurrencyConverterClientOptions.RetryOptions options)
    {
        return new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = options.MaxAttempts,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(options.InitialDelaySeconds),
            UseJitter = true,
            OnRetry = async arguments =>
            {
                var outcome = arguments.Outcome;

                if (outcome.Result is not null)
                {
                    var rawResponse = await outcome.Result.Content.ReadAsStringAsync();

                    logger.LogWarning(
                        """
                        Currency Converter API request failed with {StatusCode}.
                        Waiting {RetryDelay} before retry attempt {AttemptNumber}.
                        Raw response: {RawResponse}
                        """
                        , outcome.Result.StatusCode
                        , arguments.RetryDelay
                        , arguments.AttemptNumber
                        , rawResponse);

                    return;
                }

                logger.LogWarning(
                    """
                    Currency Converter API request failed due to a network error.
                    Waiting {RetryDelay} before retry attempt {AttemptNumber}
                    """
                    , arguments.RetryDelay
                    , arguments.AttemptNumber);
            }
        };
    }
}
