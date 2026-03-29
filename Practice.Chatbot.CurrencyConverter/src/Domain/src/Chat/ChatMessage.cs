namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

/// <summary>Immutable value object representing a single message in a conversation.</summary>
public sealed record ChatMessage
{
    public MessageRole Role { get; init; }
    public string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    private ChatMessage(MessageRole role, string content, DateTimeOffset timestamp)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be empty.", nameof(content));

        Role = role;
        Content = content;
        Timestamp = timestamp;
    }

    public static ChatMessage UserMessage(string content) =>
        new(MessageRole.User, content, DateTimeOffset.UtcNow);

    public static ChatMessage AssistantMessage(string content) =>
        new(MessageRole.Assistant, content, DateTimeOffset.UtcNow);

    public static ChatMessage SystemMessage(string content) =>
        new(MessageRole.System, content, DateTimeOffset.UtcNow);
}
