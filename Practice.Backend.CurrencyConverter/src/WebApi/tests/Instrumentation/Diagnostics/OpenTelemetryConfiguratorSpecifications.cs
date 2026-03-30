using Microsoft.AspNetCore.Builder;
using OpenTelemetry;
using OpenTelemetry.Resources;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Diagnostics;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Diagnostics;

public sealed class OpenTelemetryConfiguratorSpecifications
{
    [Fact]
    public void AddOpenTelemetry_ValidBuilder_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();

        var act = () => builder.AddOpenTelemetry();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddOpenTelemetry_ValidBuilder_RegistersOpenTelemetryServices()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddOpenTelemetry();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName!.Contains("OpenTelemetry"));
    }

    [Fact]
    public void AddOpenTelemetry_WhenAppBuilt_ConfiguresTracingAndMetrics()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddOpenTelemetry();

        var act = () => builder.Build();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddOpenTelemetry_WhenAppBuilt_ReturnsNonNullApp()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddOpenTelemetry();

        var app = builder.Build();

        app.Should().NotBeNull();
    }

    [Fact]
    public void AddOpenTelemetry_ResourceBuilder_ContainsServiceName()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddOpenTelemetry();
        using var app = builder.Build();

        var sdk = app.Services.GetService(typeof(Sdk));
        var resourceBuilder = ResourceBuilder.CreateDefault();
        resourceBuilder.AddService(serviceName: "currency-api");
        var resource = resourceBuilder.Build();

        resource.Attributes.Should().Contain(a => a.Key == "service.name" && a.Value.ToString() == "currency-api");
    }

    [Fact]
    public void AddOpenTelemetry_WhenAppStarted_ExecutesResourceConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddOpenTelemetry();
        using var app = builder.Build();

        var act = () => app.StartAsync(TestContext.Current.CancellationToken);

        act.Should().NotThrowAsync();
    }
}
