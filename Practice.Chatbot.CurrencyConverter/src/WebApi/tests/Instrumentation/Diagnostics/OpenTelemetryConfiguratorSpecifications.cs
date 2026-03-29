using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Diagnostics;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Instrumentation.Diagnostics;

public class OpenTelemetryConfiguratorSpecifications
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
