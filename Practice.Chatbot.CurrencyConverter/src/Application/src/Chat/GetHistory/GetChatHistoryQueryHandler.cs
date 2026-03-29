using MediatR;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed class GetChatHistoryQueryHandler(IChatHistoryRepository repository)
    : IRequestHandler<GetChatHistoryQuery, GetChatHistoryQueryResponse>
{
    public async Task<GetChatHistoryQueryResponse> Handle(
        GetChatHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var conversationId = ConversationId.From(request.ConversationId);
        var conversation = await repository.FindAsync(conversationId, cancellationToken);

        if (conversation is null || conversation.UserId != request.UserId)
        {
            return new GetChatHistoryQueryResponse(request.ConversationId, []);
        }

        var messages = conversation.Messages
            .Select(m => new ChatMessageDto(m.Role.ToString().ToLower(), m.Content, m.Timestamp))
            .ToList();

        return new GetChatHistoryQueryResponse(request.ConversationId, messages);
    }
}
