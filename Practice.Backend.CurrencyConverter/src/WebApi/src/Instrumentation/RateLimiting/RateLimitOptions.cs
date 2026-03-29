namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public int PermitLimit { get; init; } = 100;
    public int WindowSeconds { get; init; } = 60;
}
