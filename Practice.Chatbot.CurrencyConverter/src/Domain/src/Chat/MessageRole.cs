using Ardalis.SmartEnum;

namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

public sealed class MessageRole(string name, int value) : SmartEnum<MessageRole>(name, value)
{
    public static readonly MessageRole System = new(nameof(System), 0);
    public static readonly MessageRole User = new(nameof(User), 1);
    public static readonly MessageRole Assistant = new(nameof(Assistant), 2);
}
