using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.WebApi.Bootstrap;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Bootstrap;

public sealed class WebApplicationBuilderExtensionsSpecifications
{
    [Fact]
    public void AddCoreServices_ValidConfiguration_DoesNotThrow()
    {
        var builder = CreateBuilderWithFullConfig();

        var act = () => builder.AddCoreServices();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersHttpContextAccessor()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IHttpContextAccessor));
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersControllers()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName!.Contains("Controller"));
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersHealthChecks()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName!.Contains("HealthCheck"));
    }

    private static WebApplicationBuilder CreateBuilderWithFullConfig()
    {
        var builder = WebApplication.CreateBuilder();
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
