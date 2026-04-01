using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Extensions;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Extensions;

public sealed class ServiceCollectionExtensionsSpecifications
{
    private static IConfiguration BuildMinimalConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AI:Provider"] = "Ollama",
                ["AI:ModelId"] = "llama3",
                ["AI:Ollama:Endpoint"] = "http://localhost:11434",
                ["Redis:ConnectionString"] = "localhost:6379",
                ["Redis:InstanceName"] = "chatbot:",
                ["ChatConfiguration:ConversationTtl"] = "24:00:00",
                [$"{CurrencyConverterClientOptions.SectionName}:BaseUrl"] = "http://localhost:7263",
            })
            .Build();

    [Fact]
    public void AddInfrastructure_Always_RegistersChatHistoryRepository()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildMinimalConfig());

        services.Should().Contain(d => d.ServiceType == typeof(IChatHistoryRepository));
    }

    [Fact]
    public void AddInfrastructure_Always_RegistersChatOrchestrator()
    {
        var services = new ServiceCollection();

        services.AddInfrastructure(BuildMinimalConfig());

        services.Should().Contain(d => d.ServiceType == typeof(IChatOrchestrator));
    }

    [Fact]
    public void AddInfrastructure_Always_ConfiguresRedisOptionsFromConfiguration()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(BuildMinimalConfig());

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;

        options.Configuration.Should().Be("localhost:6379");
        options.InstanceName.Should().Be("chatbot:");
    }
}
