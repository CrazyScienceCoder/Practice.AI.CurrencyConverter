using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using MicrosoftMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.AI;

public sealed class MicrosoftAiChatOrchestrator(
    IChatClient chatClient,
    AIFunction[] tools,
    ILogger<MicrosoftAiChatOrchestrator> logger)
    : IChatOrchestrator
{
    private const string SystemPrompt = """
        You are a helpful currency conversion assistant. You have access to real-time and historical
        exchange rate data for most world currencies.

        When the user asks about exchange rates, currency conversion, or historical rates:
        1. Use the available tools to fetch accurate, up-to-date data
        2. Present the results in a clear, human-readable format
        3. Include the exchange rate and calculated amount for conversions
        4. Always mention the data source (Frankfurter API) and that rates are updated regularly

        If a tool call fails or data is unavailable, inform the user that the information could not
        be retrieved. Do NOT suggest or recommend any external websites, third-party services, or
        alternative tools (e.g. XE.com, Google, banks, or any other source). Only use the tools
        provided to you.

        If the user asks something unrelated to currencies, politely redirect the conversation
        back to currency-related topics.

        Currency codes follow the ISO 4217 standard (e.g., USD, EUR, GBP, JPY).
        Note: some currencies like TRY, PLN, THB, MXN may not be available.
        """;

    public async IAsyncEnumerable<string> StreamReplyAsync(
        Conversation conversation,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var messages = BuildMessages(conversation);
        var options = new ChatOptions { Tools = [.. tools] };

        logger.LogDebug("Streaming reply for conversation, history has {Count} messages", messages.Count);

        await foreach (var update in chatClient.GetStreamingResponseAsync(messages, options, cancellationToken))
        {
            if (update.Text is { Length: > 0 } text)
            {
                yield return text;
            }
        }
    }

    private static List<MicrosoftMessage> BuildMessages(Conversation conversation)
    {
        var messages = new List<MicrosoftMessage>
        {
            new(ChatRole.System, SystemPrompt)
        };

        foreach (var msg in conversation.Messages)
        {
            var role = msg.Role switch
            {
                var r when r == MessageRole.User => ChatRole.User,
                var r when r == MessageRole.Assistant => ChatRole.Assistant,
                _ => ChatRole.User
            };

            messages.Add(new MicrosoftMessage(role, msg.Content));
        }

        return messages;
    }
}
