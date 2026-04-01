using MediatR;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed class GetChatHistoryQuery : IRequest<GetChatHistoryQueryResponse>
{
    public required string ConversationId { get; init; }
    public required string UserId { get; init; }
}
