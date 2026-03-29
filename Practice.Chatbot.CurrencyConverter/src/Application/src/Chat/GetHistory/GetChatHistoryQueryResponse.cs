namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed record GetChatHistoryQueryResponse(
    string ConversationId,
    IReadOnlyList<ChatMessageDto> Messages
);

public sealed record ChatMessageDto(string Role, string Content, DateTimeOffset Timestamp);
