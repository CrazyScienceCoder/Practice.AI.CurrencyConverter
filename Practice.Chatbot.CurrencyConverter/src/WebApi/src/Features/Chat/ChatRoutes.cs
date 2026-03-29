namespace Practice.Chatbot.CurrencyConverter.WebApi.Features.Chat;

public static class ChatRoutes
{
    public const string BasePath = "api/v{version:apiVersion}/chat";
    public const string MessagePath = "message";
    public const string HistoryPath = "{conversationId}/history";

    public static class Version
    {
        public const string V1 = "1.0";
    }
}
