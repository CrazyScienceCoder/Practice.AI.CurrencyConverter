using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OpenAI;
using System.ClientModel;
using Practice.Backend.CurrencyConverter.Client;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Configuration;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.AI;

public static class AiClientConfigurator
{
    public static IServiceCollection AddAiClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var aiConfig = configuration.GetSection(AiConfiguration.SectionName).Get<AiConfiguration>()
                       ?? throw new InvalidOperationException($"Missing configuration section '{AiConfiguration.SectionName}'.");

        services.AddTransient(sp =>
        {
            var client = sp.GetRequiredService<ICurrencyConverterClient>();
            var logger = sp.GetRequiredService<ILogger<CurrencyConverterPlugin>>();
            return new CurrencyConverterPlugin(client, logger);
        });

        services.AddTransient<DatePlugin>();

        services.AddTransient<AIFunction[]>(sp =>
        {
            var plugin = sp.GetRequiredService<CurrencyConverterPlugin>();
            var datePlugin = sp.GetRequiredService<DatePlugin>();
            return
            [
                AIFunctionFactory.Create(
                    typeof(CurrencyConverterPlugin).GetMethod(nameof(CurrencyConverterPlugin.GetLatestExchangeRatesAsync))!,
                    plugin,
                    new AIFunctionFactoryOptions { Name = "get_latest_exchange_rates" }),

                AIFunctionFactory.Create(
                    typeof(CurrencyConverterPlugin).GetMethod(nameof(CurrencyConverterPlugin.ConvertCurrencyAsync))!,
                    plugin,
                    new AIFunctionFactoryOptions { Name = "convert_currency" }),

                AIFunctionFactory.Create(
                    typeof(CurrencyConverterPlugin).GetMethod(nameof(CurrencyConverterPlugin.GetHistoricalExchangeRatesAsync))!,
                    plugin,
                    new AIFunctionFactoryOptions { Name = "get_historical_exchange_rates" }),

                AIFunctionFactory.Create(
                    typeof(DatePlugin).GetMethod(nameof(DatePlugin.GetToday))!,
                    datePlugin,
                    new AIFunctionFactoryOptions { Name = "get_today_date" }),
            ];
        });

        services.AddChatClient(CreateInnerClient(aiConfig))
            .UseFunctionInvocation();

        return services;
    }

    private static IChatClient CreateInnerClient(AiConfiguration config)
    {
        return config.Provider.ToUpperInvariant() switch
        {
            "OLLAMA" => new OllamaApiClient(new Uri(config.Ollama.Endpoint), config.ModelId),

            "OPENAI" => new OpenAIClient(new ApiKeyCredential(config.OpenAI.ApiKey))
                .GetChatClient(config.ModelId)
                .AsIChatClient(),

            // Gemini exposes an OpenAI-compatible endpoint
            "GEMINI" => new OpenAIClient(
                    new ApiKeyCredential(config.Gemini.ApiKey),
                    new OpenAIClientOptions { Endpoint = new Uri(config.Gemini.Endpoint) })
                .GetChatClient(config.Gemini.ModelId)
                .AsIChatClient(),

            _ => throw new InvalidOperationException(
                $"Unknown AI provider '{config.Provider}'. Valid values: Ollama, OpenAI, Gemini.")
        };
    }
}
