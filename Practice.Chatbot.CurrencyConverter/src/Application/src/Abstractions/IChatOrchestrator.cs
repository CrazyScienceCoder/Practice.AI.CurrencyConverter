using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Application.Abstractions;

public interface IChatOrchestrator
{
    IAsyncEnumerable<string> StreamReplyAsync(
        Conversation conversation,
        CancellationToken cancellationToken = default);
}
