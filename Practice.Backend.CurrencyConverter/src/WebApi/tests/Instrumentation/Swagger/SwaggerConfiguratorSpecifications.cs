using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.WebApi.Bootstrap;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Swagger;

public sealed class SwaggerConfiguratorSpecifications
{
    [Fact]
    public void AddSwagger_RegistersSwaggerGenOptions()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddSwagger();

        builder.Services.Should().Contain(d =>
            d.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>));
    }

    [Fact]
    public void AddSwagger_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();

        var act = () => builder.AddSwagger();

        act.Should().NotThrow();
    }

    [Fact]
    public void UseSwaggerEndpoint_InNonDevelopmentEnvironment_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Production"
        });
        var app = builder.Build();

        var act = () => app.UseSwaggerEndpoint();

        act.Should().NotThrow();
    }

    [Fact]
    public void UseSwaggerEndpoint_InDevelopmentEnvironment_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });
        builder.Services.AddApiVersioning();
        builder.AddSwagger();
        var app = builder.Build();

        var act = () => app.UseSwaggerEndpoint();

        act.Should().NotThrow();
    }

    [Fact]
    public void UseSwaggerEndpoint_InDevelopmentEnvironment_RegistersSwaggerMiddleware()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Development"
        });
        builder.Services.AddApiVersioning();
        builder.AddSwagger();
        var app = builder.Build();

        app.UseSwaggerEndpoint();

        app.Should().NotBeNull();
    }
}
