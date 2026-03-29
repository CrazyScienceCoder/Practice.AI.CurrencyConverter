// ReSharper disable InconsistentNaming
namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Configuration;

public sealed class AiConfiguration
{
    public const string SectionName = "AI";

    public string Provider { get; init; } = string.Empty;

    public string ModelId { get; init; } = string.Empty;

    public OllamaSettings Ollama { get; init; } = new();

    public OpenAiSettings OpenAI { get; init; } = new();

    public GeminiSettings Gemini { get; init; } = new();
}

public sealed class OllamaSettings
{
    public string Endpoint { get; init; } = string.Empty;
}

public sealed class OpenAiSettings
{
    public string ApiKey { get; init; } = string.Empty;
}

public sealed class GeminiSettings
{
    public string ApiKey { get; init; } = string.Empty;

    public string ModelId { get; init; } = string.Empty;

    public string Endpoint { get; init; } = string.Empty;
}
