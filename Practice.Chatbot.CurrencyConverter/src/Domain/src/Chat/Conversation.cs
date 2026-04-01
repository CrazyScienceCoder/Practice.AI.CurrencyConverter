using Ardalis.GuardClauses;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

public sealed class Conversation
{
    private readonly List<ChatMessage> _messages = [];

    public ConversationId Id { get; }
    public string UserId { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset LastActivityAt { get; private set; }

    public IReadOnlyList<ChatMessage> Messages => _messages.AsReadOnly();

    private Conversation(ConversationId id, string userId, DateTimeOffset createdAt)
    {
        Guard.Against.NullOrWhiteSpace(userId,
            exceptionCreator: () => new InvalidUserIdException("UserId cannot be empty.", nameof(userId)));

        Id = id;
        UserId = userId;
        CreatedAt = createdAt;
        LastActivityAt = createdAt;
    }

    public static Conversation Start(string userId) =>
        new(ConversationId.New(), userId, DateTimeOffset.UtcNow);

    public static Conversation StartWithId(ConversationId id, string userId) =>
        new(id, userId, DateTimeOffset.UtcNow);

    public static Conversation Reconstitute(ConversationId id, string userId, DateTimeOffset createdAt,
        IEnumerable<ChatMessage> messages)
    {
        var conversation = new Conversation(id, userId, createdAt);

        var chatMessages = messages as ChatMessage[] ?? messages.ToArray();

        conversation._messages.AddRange(chatMessages);

        conversation.LastActivityAt = chatMessages.Length != 0
            ? chatMessages.Max(m => m.Timestamp)
            : createdAt;

        return conversation;
    }

    public void AddUserMessage(string content)
    {
        _messages.Add(ChatMessage.UserMessage(content));
        LastActivityAt = DateTimeOffset.UtcNow;
    }

    public void AddAssistantMessage(string content)
    {
        _messages.Add(ChatMessage.AssistantMessage(content));
        LastActivityAt = DateTimeOffset.UtcNow;
    }
}
