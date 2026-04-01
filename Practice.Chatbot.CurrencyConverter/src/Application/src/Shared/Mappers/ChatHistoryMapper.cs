using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Application.Shared.Mappers;

public static class ChatHistoryMapper
{
    extension(Conversation conversation)
    {
        public GetChatHistoryQueryResult ToChatHistoryResult()
        {
            return new GetChatHistoryQueryResult
            {
                ConversationId = conversation.Id.ToString(),
                Messages = conversation.Messages
                    .Select(m => new ChatMessageDto
                    {
                        Role = m.Role.ToString().ToLower(),
                        Content = m.Content,
                        Timestamp = m.Timestamp
                    })
                    .ToList()
            };
        }
    }
}
