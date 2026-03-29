using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;
using Practice.Chatbot.CurrencyConverter.Application.Chat.Send;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Chat.Send;

public partial class SendChatMessageCommandHandlerSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IChatHistoryRepository> RepositoryMock = new();
        public readonly Mock<IChatOrchestrator> OrchestratorMock = new();
        private readonly Mock<ILogger<SendChatMessageCommandHandler>> _loggerMock = new();

        public Conversation? CapturedConversation { get; private set; }

        private static readonly string DefaultConversationId = Guid.NewGuid().ToString();
        private const string DefaultUserId = "user-123";

        public readonly SendChatMessageCommand NewConversationCommand = new(
            ConversationId: null,
            UserId: DefaultUserId,
            UserMessage: "What is the USD exchange rate?");

        public readonly SendChatMessageCommand ExistingConversationCommand;

        public TestBuilder()
        {
            ExistingConversationCommand = new(
                ConversationId: DefaultConversationId,
                UserId: DefaultUserId,
                UserMessage: "What about EUR?");
        }

        public TestBuilder SetupNewConversation(params string[] replyChunks)
        {
            SetupOrchestrator(replyChunks.Length > 0 ? replyChunks : ["The current exchange rate is 1.0."]);
            SetupSaveConversation();
            return this;
        }

        public TestBuilder SetupExistingConversationFound(params string[] replyChunks)
        {
            var conversationId = ConversationId.From(DefaultConversationId);
            var existing = Conversation.StartWithId(conversationId, DefaultUserId);
            existing.AddUserMessage("Previous question.");
            existing.AddAssistantMessage("Previous answer.");

            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existing)
                .Verifiable(Times.Once);

            SetupOrchestrator(replyChunks.Length > 0 ? replyChunks : ["The EUR exchange rate is 0.92."]);
            SetupSaveConversation();
            return this;
        }

        public TestBuilder SetupExistingConversationNotFound(params string[] replyChunks)
        {
            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Conversation?)null)
                .Verifiable(Times.Once);

            SetupOrchestrator(replyChunks.Length > 0 ? replyChunks : ["Starting a fresh conversation."]);
            SetupSaveConversation();
            return this;
        }

        private void SetupOrchestrator(string[] chunks)
        {
            OrchestratorMock
                .Setup(o => o.StreamReplyAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(chunks))
                .Verifiable(Times.Once);
        }

        private void SetupSaveConversation()
        {
            RepositoryMock
                .Setup(r => r.SaveAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
                .Callback<Conversation, CancellationToken>((conv, _) => CapturedConversation = conv)
                .Returns(Task.CompletedTask)
                .Verifiable(Times.Once);
        }

        public SendChatMessageCommandHandler Build()
            => new(RepositoryMock.Object, OrchestratorMock.Object, _loggerMock.Object);

        private static async IAsyncEnumerable<string> ToAsyncEnumerable(string[] chunks)
        {
            foreach (var chunk in chunks)
                yield return chunk;
        }
    }
}
