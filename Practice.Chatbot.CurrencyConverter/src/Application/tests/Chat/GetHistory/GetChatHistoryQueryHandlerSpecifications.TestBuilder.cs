using Microsoft.Extensions.Logging;
using Practice.Chatbot.CurrencyConverter.Application.Chat.GetHistory;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using Practice.Chatbot.CurrencyConverter.Domain.Contracts;

namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Chat.GetHistory;

public partial class GetChatHistoryQueryHandlerSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<IChatHistoryRepository> RepositoryMock = new();
        private readonly Mock<ILogger<GetChatHistoryQueryHandler>> _loggerMock = new();

        private static readonly ConversationId DefaultConversationId = ConversationId.New();
        private const string DefaultUserId = "user-123";

        public readonly GetChatHistoryQuery DefaultQuery = new()
        {
            ConversationId = DefaultConversationId.ToString(),
            UserId = DefaultUserId
        };

        public TestBuilder SetupConversationFound()
        {
            var conversation = Conversation.StartWithId(DefaultConversationId, DefaultUserId);
            conversation.AddUserMessage("What is the exchange rate for USD?");
            conversation.AddAssistantMessage("The current USD exchange rate is 1.0.");

            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversation)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupConversationFoundWithNoMessages()
        {
            var conversation = Conversation.StartWithId(DefaultConversationId, DefaultUserId);

            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversation)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupConversationNotFound()
        {
            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Conversation?)null)
                .Verifiable(Times.Once);

            return this;
        }

        public TestBuilder SetupConversationBelongsToDifferentUser()
        {
            var differentUserId = "different-user-456";
            var conversation = Conversation.StartWithId(DefaultConversationId, differentUserId);
            conversation.AddUserMessage("Some message from a different user.");

            RepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<ConversationId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(conversation)
                .Verifiable(Times.Once);

            return this;
        }

        public GetChatHistoryQueryHandler Build()
            => new(RepositoryMock.Object, _loggerMock.Object);
    }
}
