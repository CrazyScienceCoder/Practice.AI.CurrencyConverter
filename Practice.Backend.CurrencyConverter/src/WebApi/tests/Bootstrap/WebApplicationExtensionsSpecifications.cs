using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.WebApi.Bootstrap;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Bootstrap;

public sealed class WebApplicationExtensionsSpecifications
{
    [Fact]
    public void ConfigurePipeline_ValidApp_DoesNotThrow()
    {
        var builder = CreateBuilderWithFullConfig();
        builder.UseSerilog();
        builder.AddCoreServices();
        var app = builder.Build();

        var act = () => app.ConfigurePipeline();

        act.Should().NotThrow();
    }

    [Fact]
    public void ConfigurePipeline_ValidApp_RegistersHealthCheckEndpoint()
    {
        var builder = CreateBuilderWithFullConfig();
        builder.UseSerilog();
        builder.AddCoreServices();
        var app = builder.Build();

        app.ConfigurePipeline();

        app.Services.GetService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
            .Should().NotBeNull();
    }

    private static WebApplicationBuilder CreateBuilderWithFullConfig()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Production"
        });

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["JwtAuth:Authority"] = "http://localhost:8080/realms/currency-converter",
            ["JwtAuth:Audience"] = "currency-api",
            ["JwtAuth:RequireHttpsMetadata"] = "false",
            ["Redis:ConnectionString"] = "localhost:6379",
            ["Redis:InstanceName"] = "CurrencyConverter",
            ["CacheConfiguration:LatestRatesTtl"] = "24:00:00",
            ["CacheConfiguration:HistoricalRatesTtl"] = "720:00:00"
        });

        return builder;
    }
}
