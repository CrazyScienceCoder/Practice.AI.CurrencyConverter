using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Extensions;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;
using Practice.Chatbot.CurrencyConverter.Infrastructure.AI;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Chat;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Configurations;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var redis = configuration.GetRequiredSection(nameof(Redis)).Get<Redis>()!;

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redis.ConnectionString;
            options.InstanceName = redis.InstanceName;
        });

        services.Configure<ChatConfiguration>(configuration.GetRequiredSection(nameof(ChatConfiguration)));

        services.AddTransient<IChatHistoryRepository, RedisChatHistoryRepository>();
        services.AddTransient<IChatOrchestrator, MicrosoftAiChatOrchestrator>();

        services.AddDefaultHttpContextTokenProvider();
        services.AddCurrencyConverterClient(configuration.GetSection(CurrencyConverterClientOptions.SectionName));

        services.AddAiClient(configuration);
    }
}
