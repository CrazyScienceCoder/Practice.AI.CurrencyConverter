using Ardalis.GuardClauses;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

public sealed record ChatMessage
{
    public MessageRole Role { get; init; }
    public string Content { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    private ChatMessage(MessageRole role, string content, DateTimeOffset timestamp)
    {
        Guard.Against.NullOrWhiteSpace(content,
            exceptionCreator: () => new InvalidMessageContentException("Message content cannot be empty.", nameof(content)));

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
