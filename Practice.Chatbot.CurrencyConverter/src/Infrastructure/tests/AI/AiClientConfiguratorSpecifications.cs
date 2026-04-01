using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Client;
using Practice.Chatbot.CurrencyConverter.Infrastructure.AI;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.AI;

public sealed class AiClientConfiguratorSpecifications
{
    [Fact]
    public void AddAiClient_MissingAiConfigSection_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var config = new ConfigurationBuilder().Build(); // empty — no AI section

        var act = () => services.AddAiClient(config);

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("*AI*");
    }

    [Fact]
    public void AddAiClient_UnknownProvider_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("Unknown", "some-model");

        var act = () => services.AddAiClient(config);

        act.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("*Unknown*");
    }

    [Fact]
    public void AddAiClient_OllamaProvider_DoesNotThrow()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("Ollama", "llama3", ollama: "http://localhost:11434");

        var act = () => services.AddAiClient(config);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddAiClient_OllamaProvider_RegistersChatClientInServiceCollection()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("Ollama", "llama3", ollama: "http://localhost:11434");

        services.AddAiClient(config);

        services.Should().Contain(d => d.ServiceType == typeof(IChatClient));
    }

    [Fact]
    public void AddAiClient_OpenAiProvider_DoesNotThrow()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("OpenAI", "gpt-4o", openAiApiKey: "sk-test-key");

        var act = () => services.AddAiClient(config);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddAiClient_OpenAiProvider_RegistersChatClientInServiceCollection()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("OpenAI", "gpt-4o", openAiApiKey: "sk-test-key");

        services.AddAiClient(config);

        services.Should().Contain(d => d.ServiceType == typeof(IChatClient));
    }

    [Fact]
    public void AddAiClient_GeminiProvider_DoesNotThrow()
    {
        var services = new ServiceCollection();
        var config = BuildConfig(
            "Gemini",
            "gemini-1.5-pro",
            geminiApiKey: "gemini-key",
            geminiEndpoint: "https://generativelanguage.googleapis.com/v1beta/openai/",
            geminiModelId: "gemini-1.5-pro");

        var act = () => services.AddAiClient(config);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddAiClient_GeminiProvider_RegistersChatClientInServiceCollection()
    {
        var services = new ServiceCollection();
        var config = BuildConfig(
            "Gemini",
            "gemini-1.5-pro",
            geminiApiKey: "gemini-key",
            geminiEndpoint: "https://generativelanguage.googleapis.com/v1beta/openai/",
            geminiModelId: "gemini-1.5-pro");

        services.AddAiClient(config);

        services.Should().Contain(d => d.ServiceType == typeof(IChatClient));
    }

    [Fact]
    public void AddAiClient_OllamaProvider_RegistersDatePluginTransient()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("Ollama", "llama3", ollama: "http://localhost:11434");

        services.AddAiClient(config);

        services.Should().Contain(d =>
            d.ServiceType == typeof(Infrastructure.Plugins.DatePlugin) &&
            d.Lifetime == ServiceLifetime.Transient);
    }

    [Fact]
    public void AddAiClient_OllamaProvider_AiFunctionArrayFactoryResolvesFourFunctions()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<ICurrencyConverterClient>());

        var config = BuildConfig("Ollama", "llama3", ollama: "http://localhost:11434");
        services.AddAiClient(config);

        using var provider = services.BuildServiceProvider();
        var functions = provider.GetRequiredService<AIFunction[]>();

        functions.Should().HaveCount(4);
    }

    [Fact]
    public void AddAiClient_OllamaProvider_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();
        var config = BuildConfig("Ollama", "llama3", ollama: "http://localhost:11434");

        var result = services.AddAiClient(config);

        result.Should().BeSameAs(services);
    }

    private static IConfiguration BuildConfig(
        string provider,
        string modelId,
        string? ollama = null,
        string? openAiApiKey = null,
        string? geminiApiKey = null,
        string? geminiEndpoint = null,
        string? geminiModelId = null)
    {
        var dict = new Dictionary<string, string?>
        {
            ["AI:Provider"] = provider,
            ["AI:ModelId"] = modelId,
            ["AI:Ollama:Endpoint"] = ollama ?? string.Empty,
            ["AI:OpenAI:ApiKey"] = openAiApiKey ?? string.Empty,
            ["AI:Gemini:ApiKey"] = geminiApiKey ?? string.Empty,
            ["AI:Gemini:Endpoint"] = geminiEndpoint ?? string.Empty,
            ["AI:Gemini:ModelId"] = geminiModelId ?? string.Empty,
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .Build();
    }
}
