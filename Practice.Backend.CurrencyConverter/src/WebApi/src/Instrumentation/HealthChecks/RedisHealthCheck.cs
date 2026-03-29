using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.HealthChecks;

public sealed class RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var db = connectionMultiplexer.GetDatabase();
            var latency = await db.PingAsync();
            return HealthCheckResult.Healthy($"Redis latency: {latency.TotalMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unreachable.", ex);
        }
    }
}
