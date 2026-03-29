using Microsoft.AspNetCore.Builder;
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
}
