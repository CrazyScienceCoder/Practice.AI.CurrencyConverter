using Practice.Backend.CurrencyConverter.WebApi.Extensions;
using RedisRateLimiting;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;

public static class RateLimitingConfigurator
{
    public const string PolicyName = "per-user";

    public static void AddRateLimiting(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration
            .GetSection(RateLimitOptions.SectionName)
            .Get<RateLimitOptions>() ?? new RateLimitOptions();

        builder.Services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            limiterOptions.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response
                    .WriteAsync("Too many requests. Please try again later.", cancellationToken);
            };

            limiterOptions.AddPolicy<string>(PolicyName, httpContext =>
            {
                var userId = httpContext.User.GetUserId();

                var multiplexer = httpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();

                return RedisRateLimitPartition.GetSlidingWindowRateLimiter(
                    partitionKey: userId,
                    factory: _ => new RedisSlidingWindowRateLimiterOptions
                    {
                        ConnectionMultiplexerFactory = () => multiplexer,
                        PermitLimit = options.PermitLimit,
                        Window = TimeSpan.FromSeconds(options.WindowSeconds)
                    });
            });
        });
    }
}
