namespace Practice.Chatbot.CurrencyConverter.Domain.Chat;

/// <summary>Strongly-typed identifier for a <see cref="Conversation"/>.</summary>
public sealed record ConversationId(Guid Value)
{
    public static ConversationId New() => new(Guid.NewGuid());

    public static ConversationId From(string value) =>
        Guid.TryParse(value, out var id)
            ? new ConversationId(id)
            : throw new ArgumentException($"'{value}' is not a valid ConversationId.", nameof(value));

    public override string ToString() => Value.ToString();
}
