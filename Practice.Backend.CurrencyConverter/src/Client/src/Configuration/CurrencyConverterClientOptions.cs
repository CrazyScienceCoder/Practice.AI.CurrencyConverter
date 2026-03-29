using System.ComponentModel.DataAnnotations;

namespace Practice.Backend.CurrencyConverter.Client.Configuration;

public sealed class CurrencyConverterClientOptions
{
    public const string SectionName = "CurrencyConverterClient";

    [Required]
    [Url]
    public string BaseUrl { get; set; } = null!;

    public string ApiVersion { get; set; } = "1";

    [Range(1, 600)]
    public int TimeoutSeconds { get; set; } = 30;

    public RetryOptions Retry { get; set; } = new();

    public CircuitBreakerOptions CircuitBreaker { get; set; } = new();

    public sealed class RetryOptions
    {
        [Range(0, 10)]
        public int MaxAttempts { get; set; } = 3;

        [Range(1, 60)]
        public int InitialDelaySeconds { get; set; } = 2;
    }

    public sealed class CircuitBreakerOptions
    {
        [Range(0.0, 1.0)]
        public double FailureRatio { get; set; } = 0.5;

        [Range(1, 3600)]
        public int BreakDurationSeconds { get; set; } = 60;

        [Range(1, 3600)]
        public int SamplingDurationSeconds { get; set; } = 60;

        [Range(1, 1000)]
        public int MinimumThroughput { get; set; } = 30;
    }
}
