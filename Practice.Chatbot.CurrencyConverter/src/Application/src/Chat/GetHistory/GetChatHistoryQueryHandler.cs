using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Application.Shared;
using Practice.Chatbot.CurrencyConverter.Application.Shared.Mappers;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;

public sealed class GetChatHistoryQueryHandler(
    IChatHistoryRepository repository,
    ILogger<GetChatHistoryQueryHandler> logger)
    : HandlerBase<GetChatHistoryQuery, GetChatHistoryQueryResponse, GetChatHistoryQueryResult>(logger)
{
    protected override async Task<GetChatHistoryQueryResponse> ExecuteAsync(
        GetChatHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var conversationId = ConversationId.From(request.ConversationId);
        var conversation = await repository.FindAsync(conversationId, cancellationToken);

        if (conversation is null || conversation.UserId != request.UserId)
        {
            return GetChatHistoryQueryResponse.Failure(
                errorType: ErrorType.NotFound,
                message: $"Conversation not found, ConversationId: {request.ConversationId}");
        }

        return GetChatHistoryQueryResponse.Success(
            data: conversation.ToChatHistoryResult(),
            message: "Chat history retrieved successfully");
    }
}
