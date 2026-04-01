namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Configurations;

public sealed class ChatConfiguration
{
    public TimeSpan ConversationTtl { get; set; } = TimeSpan.FromHours(24);
}
