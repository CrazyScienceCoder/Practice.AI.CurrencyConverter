using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Client.Configuration;

namespace Practice.Backend.CurrencyConverter.Client.Resilience;

internal static class HttpCircuitBreakerStrategy
{
    internal static HttpCircuitBreakerStrategyOptions Create(
        ILogger logger,
        CurrencyConverterClientOptions.CircuitBreakerOptions options)
    {
        return new HttpCircuitBreakerStrategyOptions
        {
            FailureRatio = options.FailureRatio,
            SamplingDuration = TimeSpan.FromSeconds(options.SamplingDurationSeconds),
            MinimumThroughput = options.MinimumThroughput,
            BreakDuration = TimeSpan.FromSeconds(options.BreakDurationSeconds),
            OnOpened = arguments =>
            {
                logger.LogError(
                    "Currency Converter API circuit breaker OPENED for host {Host}. " +
                    "Break duration: {BreakDuration}. Service calls will be rejected.",
                    arguments.Outcome.Result?.RequestMessage?.RequestUri?.Host,
                    arguments.BreakDuration);

                return ValueTask.CompletedTask;
            },
            OnClosed = arguments =>
            {
                logger.LogInformation(
                    "Currency Converter API circuit breaker CLOSED for host {Host}. Service calls resumed.",
                    arguments.Outcome.Result?.RequestMessage?.RequestUri?.Host);

                return ValueTask.CompletedTask;
            }
        };
    }
}
