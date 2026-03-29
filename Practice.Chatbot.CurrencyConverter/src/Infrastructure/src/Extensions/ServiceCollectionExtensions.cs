using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Extensions;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;
using Practice.Chatbot.CurrencyConverter.Infrastructure.AI;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Chat;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            var redis = configuration.GetSection("Redis");
            options.Configuration = redis["ConnectionString"];
            options.InstanceName = redis["InstanceName"];
        });

        services.AddScoped<IChatHistoryRepository, RedisChatHistoryRepository>();
        services.AddScoped<IChatOrchestrator, MicrosoftAiChatOrchestrator>();

        services.AddDefaultHttpContextTokenProvider();
        services.AddCurrencyConverterClient(configuration.GetSection(CurrencyConverterClientOptions.SectionName));

        services.AddAiClient(configuration);

        return services;
    }
}
