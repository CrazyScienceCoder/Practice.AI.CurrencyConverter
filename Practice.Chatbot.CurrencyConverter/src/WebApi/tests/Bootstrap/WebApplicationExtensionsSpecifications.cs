using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Practice.Chatbot.CurrencyConverter.WebApi.Bootstrap;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Logging;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Bootstrap;

public class WebApplicationExtensionsSpecifications
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
    public void ConfigurePipeline_ValidApp_RegistersHealthCheckServices()
    {
        var builder = CreateBuilderWithFullConfig();
        builder.UseSerilog();
        builder.AddCoreServices();
        var app = builder.Build();

        app.ConfigurePipeline();

        app.Services.GetService<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>()
            .Should().NotBeNull();
    }

    /// <summary>
    /// Creates a builder with cleared config sources so that appsettings.json
    /// (copied to test output) does not load the OpenSearch Serilog sink with
    /// an unresolved %OPENSEARCH_NODE_URIS% URI placeholder.
    /// </summary>
    private static WebApplicationBuilder CreateBuilderWithFullConfig()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Production"
        });

        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false",
            ["Redis:ConnectionString"] = "localhost:6379",
            ["Redis:InstanceName"] = "ChatBot",
            ["CurrencyConverterClient:BaseUrl"] = "http://localhost:5263",
            ["CurrencyConverterClient:ApiVersion"] = "1.0",
            ["AI:Provider"] = "Ollama",
            ["AI:ModelId"] = "test-model",
            ["AI:Ollama:Endpoint"] = "http://localhost:11434"
        });

        return builder;
    }
}
