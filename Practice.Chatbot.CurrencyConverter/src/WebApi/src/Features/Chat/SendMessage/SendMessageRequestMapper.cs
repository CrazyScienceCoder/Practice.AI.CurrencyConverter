using Practice.Chatbot.CurrencyConverter.Application.Chat.Send;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat.SendMessage;

public static class SendMessageRequestMapper
{
    public static SendChatMessageCommand ToCommand(this SendMessageRequest request, string userId)
    {
        return new SendChatMessageCommand
        {
            ConversationId = request.ConversationId,
            UserId = userId,
            UserMessage = request.Message
        };
    }
}
