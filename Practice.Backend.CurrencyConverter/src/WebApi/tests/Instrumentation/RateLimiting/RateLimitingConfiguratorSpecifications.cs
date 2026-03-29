using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.RateLimiting;

public sealed class RateLimitingConfiguratorSpecifications
{
    [Fact]
    public void PolicyName_HasCorrectValue()
    {
        RateLimitingConfigurator.PolicyName.Should().Be("per-user");
    }

    [Fact]
    public void AddRateLimiting_WithMissingSection_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);

        var act = () => builder.AddRateLimiting();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddRateLimiting_WithValidConfiguration_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimit:PermitLimit"] = "50",
            ["RateLimit:WindowSeconds"] = "30"
        });

        var act = () => builder.AddRateLimiting();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddRateLimiting_ValidConfiguration_RegistersRateLimiterRelatedServices()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);

        builder.AddRateLimiting();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("RateLimit"));
    }
}
