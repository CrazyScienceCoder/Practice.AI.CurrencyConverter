using Ardalis.GuardClauses;
using Practice.Chatbot.CurrencyConverter.Domain.Exceptions;

namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

public sealed record ConversationId(Guid Value)
{
    public static ConversationId New() => new(Guid.NewGuid());

    public static ConversationId From(string value)
    {
        Guard.Against.InvalidInput(value, nameof(value), v => Guid.TryParse(v, out _),
            exceptionCreator: () => new InvalidConversationIdException($"'{value}' is not a valid ConversationId.", nameof(value)));

        return new ConversationId(Guid.Parse(value));
    }

    public static implicit operator ConversationId(Guid value) => new(value);
    public static implicit operator Guid(ConversationId id) => id.Value;

    public override string ToString() => Value.ToString();
}
