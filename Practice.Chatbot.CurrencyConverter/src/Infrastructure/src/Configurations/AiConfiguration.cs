// ReSharper disable InconsistentNaming
namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Configurations;

public sealed class AiConfiguration
{
    public const string SectionName = "AI";

    public required string Provider { get; set; }

    public required string ModelId { get; set; }

    public OllamaSettings Ollama { get; set; } = new();

    public OpenAiSettings OpenAI { get; set; } = new();

    public GeminiSettings Gemini { get; set; } = new();
}

public sealed class OllamaSettings
{
    public string Endpoint { get; set; } = string.Empty;
}

public sealed class OpenAiSettings
{
    public string ApiKey { get; set; } = string.Empty;
}

public sealed class GeminiSettings
{
    public string ApiKey { get; set; } = string.Empty;

    public string ModelId { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;
}
