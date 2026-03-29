namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

public sealed record SendMessageRequest(
    string? ConversationId,
    string Message
);
