using MediatR;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed record GetChatHistoryQuery(string ConversationId, string UserId)
    : IRequest<GetChatHistoryQueryResponse>;
