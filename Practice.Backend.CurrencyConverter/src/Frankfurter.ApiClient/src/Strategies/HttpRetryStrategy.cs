using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Strategies;

public static class HttpRetryStrategy
{
    private const int RetryCount = 3;
    private const int InitialDelaySeconds = 2;

    public static HttpRetryStrategyOptions Create(ILogger logger)
    {
        return new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = RetryCount,
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(InitialDelaySeconds),
            UseJitter = true,
            OnRetry = async arguments =>
            {
                var outcome = arguments.Outcome;

                if (outcome.Result is not null)
                {
                    var rawResponse = JsonConvert.SerializeObject(await outcome.Result.Content.ReadAsStringAsync());

                    logger.LogWarning(
                        """
                        Request failed with {StatusCode}.
                        Waiting {RetryDelay} before next retry.
                        Retry attempt {AttemptNumber}.
                        Raw response {RawResponse}
                        """
                        , outcome.Result.StatusCode
                        , arguments.RetryDelay
                        , arguments.AttemptNumber
                        , rawResponse);

                    return;
                }

                logger.LogWarning(
                    """
                    Request failed because network failure.
                    Waiting {RetryDelay} before next retry.
                    Retry attempt {AttemptNumber}
                    """
                    , arguments.RetryDelay
                    , arguments.AttemptNumber);
            }
        };
    }
}
