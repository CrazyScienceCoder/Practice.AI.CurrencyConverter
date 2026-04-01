using MediatR;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.Send;

public sealed class SendChatMessageCommand : IStreamRequest<string>
{
    public string? ConversationId { get; init; }
    public required string UserId { get; init; }
    public required string UserMessage { get; init; }
}
