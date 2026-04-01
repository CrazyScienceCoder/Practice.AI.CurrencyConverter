using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Chatbot.CurrencyConverter.WebApi.Bootstrap;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Bootstrap;

public class WebApplicationBuilderExtensionsSpecifications
{
    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersHttpContextAccessor()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IHttpContextAccessor));
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersHealthChecks()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName!.Contains("HealthCheck"));
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_RegistersCors()
    {
        var builder = CreateBuilderWithFullConfig();

        builder.AddCoreServices();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName!.Contains("Cors"));
    }

    [Fact]
    public void AddCoreServices_ValidConfiguration_DoesNotThrow()
    {
        var builder = CreateBuilderWithFullConfig();

        var act = () => builder.AddCoreServices();

        act.Should().NotThrow();
    }

    private static WebApplicationBuilder CreateBuilderWithFullConfig()
    {
        var builder = WebApplication.CreateBuilder();
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
            ["AI:Ollama:Endpoint"] = "http://localhost:11434",
            ["ChatConfiguration:ConversationTtl"] = "24:00:00"
        });
        return builder;
    }
}
