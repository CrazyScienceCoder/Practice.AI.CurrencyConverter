using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Strategies;

public static class HttpCircuitBreakerStrategy
{
    private const double FailureRatio = 0.5;
    private const int BreakDurationSeconds = 60;
    private const int SamplingDurationSeconds = 60;
    private const int MinimumThroughput = 30;

    public static HttpCircuitBreakerStrategyOptions Create(ILogger logger)
    {
        return new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = FailureRatio,
            SamplingDuration = TimeSpan.FromSeconds(SamplingDurationSeconds),
            MinimumThroughput = MinimumThroughput,
            BreakDuration = TimeSpan.FromSeconds(BreakDurationSeconds),
            OnOpened = arguments =>
            {
                logger.LogError(
                    """
                    Host: {HostUri} Service shutdown,
                    BreakDuration: {BreakDuration},
                    Circuit breaker has transitioned to the OPEN state.
                    """
                    , arguments.Outcome.Result?.RequestMessage?.RequestUri?.Host
                    , arguments.BreakDuration);

                return ValueTask.CompletedTask;
            },
            OnClosed = arguments =>
            {
                logger.LogInformation(
                    "Host: {HostUri} Service restarted, Circuit breaker has transitioned to the CLOSED state."
                    , arguments.Outcome.Result?.RequestMessage?.RequestUri?.Host);

                return ValueTask.CompletedTask;
            }
        };
    }
}
