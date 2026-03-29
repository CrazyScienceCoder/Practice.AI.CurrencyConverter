using Asp.Versioning;
using Microsoft.Extensions.Options;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Instrumentation.Swagger;

public class SwaggerConfiguratorSpecifications
{
    [Fact]
    public void AddSwagger_RegistersSwaggerGenOptions()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApiVersioning().AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.AddSwagger();

        builder.Services.Should().Contain(d =>
            d.ServiceType == typeof(IConfigureOptions<SwaggerGenOptions>));
    }

    [Fact]
    public void AddSwagger_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddApiVersioning().AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });

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
}
