using MediatR;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.Send;

public sealed record SendChatMessageCommand(
    string? ConversationId,
    string UserId,
    string UserMessage
) : IStreamRequest<string>;
