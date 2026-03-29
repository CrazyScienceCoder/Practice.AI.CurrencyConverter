using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Serilog;

namespace Practice.Backend.CurrencyConverter.WebApi.Bootstrap;

public static class WebApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseSwaggerEndpoint();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers().RequireRateLimiting(RateLimitingConfigurator.PolicyName);

        app.MapHealthChecks("/health");
    }
}