using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Domain.Contracts;

/// <summary>Port for persisting and retrieving conversation history.</summary>
public interface IChatHistoryRepository
{
    Task<Conversation?> FindAsync(ConversationId id, CancellationToken cancellationToken = default);

    Task SaveAsync(Conversation conversation, CancellationToken cancellationToken = default);
}
