namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed class GetChatHistoryQueryResult
{
    public required string ConversationId { get; init; }
    public required IReadOnlyList<ChatMessageDto> Messages { get; init; }
}

public sealed class ChatMessageDto
{
    public required string Role { get; init; }
    public required string Content { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
}
