using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Logging;

public sealed class SerilogConfiguratorSpecifications
{
    [Fact]
    public void CreateBootstrapLogger_WithEmptyConfiguration_ReturnsNonNullLogger()
    {
        var configuration = new ConfigurationBuilder().Build();

        var logger = SerilogConfigurator.CreateBootstrapLogger(configuration);

        logger.Should().NotBeNull();
    }

    [Fact]
    public void UseSerilog_ValidBuilder_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();

        var act = () => builder.UseSerilog();

        act.Should().NotThrow();
    }
}
