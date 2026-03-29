using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Infrastructure.AI;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.AI;

public sealed partial class MicrosoftAiChatOrchestratorSpecifications
{
    private class TestBuilder
    {
        public Mock<IChatClient> ChatClientMock { get; } = new();
        private readonly Mock<ILogger<MicrosoftAiChatOrchestrator>> _loggerMock = new();

        public TestBuilder WithStreamingUpdates(params string[] textChunks)
        {
            var updates = textChunks
                .Select(t => new ChatResponseUpdate((ChatRole?)null, t))
                .ToArray();

            ChatClientMock
                .Setup(c => c.GetStreamingResponseAsync(
                    It.IsAny<IEnumerable<AiChatMessage>>(),
                    It.IsAny<ChatOptions?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(AsAsyncEnumerable(updates));

            return this;
        }

        public TestBuilder WithEmptyStream()
        {
            ChatClientMock
                .Setup(c => c.GetStreamingResponseAsync(
                    It.IsAny<IEnumerable<AiChatMessage>>(),
                    It.IsAny<ChatOptions?>(),
                    It.IsAny<CancellationToken>()))
                .Returns(AsAsyncEnumerable([]));

            return this;
        }

        public MicrosoftAiChatOrchestrator Build() =>
            new(ChatClientMock.Object, [], _loggerMock.Object);

        public static async IAsyncEnumerable<ChatResponseUpdate> AsAsyncEnumerable(
            ChatResponseUpdate[] items,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return item;
            }
        }
    }

    private static async Task<List<string>> CollectChunksAsync(
        MicrosoftAiChatOrchestrator sut,
        Domain.Chat.Conversation conversation,
        CancellationToken cancellationToken = default)
    {
        var chunks = new List<string>();
        await foreach (var chunk in sut.StreamReplyAsync(conversation, cancellationToken))
            chunks.Add(chunk);
        return chunks;
    }
}
