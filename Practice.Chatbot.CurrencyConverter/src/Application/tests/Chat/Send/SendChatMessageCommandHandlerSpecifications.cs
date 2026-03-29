using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Chat.Send;

public partial class SendChatMessageCommandHandlerSpecifications
{
    [Fact]
    public async Task Handle_NewConversation_StreamsAllChunksFromOrchestrator()
    {
        var testBuilder = new TestBuilder()
            .SetupNewConversation("Hello, ", "how can I help?");

        var handler = testBuilder.Build();

        var chunks = await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        testBuilder.OrchestratorMock.Verify();
        chunks.Should().HaveCount(2);
        chunks.Should().ContainInOrder("Hello, ", "how can I help?");
    }

    [Fact]
    public async Task Handle_NewConversation_SavesConversationAfterStreaming()
    {
        var testBuilder = new TestBuilder()
            .SetupNewConversation("AI response.");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        testBuilder.RepositoryMock.Verify();
    }

    [Fact]
    public async Task Handle_NewConversation_SavedConversationContainsUserAndAssistantMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupNewConversation("AI response.");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        testBuilder.CapturedConversation.Should().NotBeNull();
        testBuilder.CapturedConversation!.Messages.Should().HaveCount(2);
        testBuilder.CapturedConversation.Messages[0].Role.Should().Be(MessageRole.User);
        testBuilder.CapturedConversation.Messages[0].Content.Should().Be(testBuilder.NewConversationCommand.UserMessage);
        testBuilder.CapturedConversation.Messages[1].Role.Should().Be(MessageRole.Assistant);
    }

    [Fact]
    public async Task Handle_NewConversation_SavedConversationAssistantMessageIsFullReply()
    {
        var testBuilder = new TestBuilder()
            .SetupNewConversation("chunk1", " chunk2", " chunk3");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        testBuilder.CapturedConversation.Should().NotBeNull();
        testBuilder.CapturedConversation!.Messages[1].Content.Should().Be("chunk1 chunk2 chunk3");
    }

    [Fact]
    public async Task Handle_NewConversation_SavedConversationBelongsToCorrectUser()
    {
        var testBuilder = new TestBuilder()
            .SetupNewConversation("Response.");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        testBuilder.CapturedConversation.Should().NotBeNull();
        testBuilder.CapturedConversation!.UserId.Should().Be(testBuilder.NewConversationCommand.UserId);
    }

    [Fact]
    public async Task Handle_ExistingConversationFound_LoadsConversationAndStreamsReply()
    {
        var testBuilder = new TestBuilder()
            .SetupExistingConversationFound("EUR reply.");

        var handler = testBuilder.Build();

        var chunks = await CollectChunksAsync(handler, testBuilder.ExistingConversationCommand);

        testBuilder.RepositoryMock.Verify();
        testBuilder.OrchestratorMock.Verify();
        chunks.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ExistingConversationFound_SavedConversationIncludesPreviousAndNewMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupExistingConversationFound("EUR reply.");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.ExistingConversationCommand);

        // 2 previous messages + 1 new user + 1 new assistant = 4 total
        testBuilder.CapturedConversation.Should().NotBeNull();
        testBuilder.CapturedConversation!.Messages.Should().HaveCount(4);
    }

    [Fact]
    public async Task Handle_ExistingConversationIdButNotFound_StartsNewConversationWithProvidedId()
    {
        var testBuilder = new TestBuilder()
            .SetupExistingConversationNotFound("Starting fresh.");

        var handler = testBuilder.Build();

        var chunks = await CollectChunksAsync(handler, testBuilder.ExistingConversationCommand);

        testBuilder.RepositoryMock.Verify();
        testBuilder.OrchestratorMock.Verify();
        chunks.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ExistingConversationIdButNotFound_SavedConversationHasCorrectId()
    {
        var testBuilder = new TestBuilder()
            .SetupExistingConversationNotFound("Response.");

        var handler = testBuilder.Build();

        await CollectChunksAsync(handler, testBuilder.ExistingConversationCommand);

        testBuilder.CapturedConversation.Should().NotBeNull();
        testBuilder.CapturedConversation!.Id.ToString()
            .Should().Be(testBuilder.ExistingConversationCommand.ConversationId);
    }

    [Fact]
    public async Task Handle_MultipleChunksFromOrchestrator_YieldsEachChunkInOrder()
    {
        var expectedChunks = new[] { "The ", "rate ", "is 1.0." };

        var testBuilder = new TestBuilder()
            .SetupNewConversation(expectedChunks);

        var handler = testBuilder.Build();

        var receivedChunks = await CollectChunksAsync(handler, testBuilder.NewConversationCommand);

        receivedChunks.Should().ContainInOrder(expectedChunks);
    }

    private static async Task<List<string>> CollectChunksAsync(
        Application.Chat.Send.SendChatMessageCommandHandler handler,
        Application.Chat.Send.SendChatMessageCommand command)
    {
        var chunks = new List<string>();
        await foreach (var chunk in handler.Handle(command, CancellationToken.None))
            chunks.Add(chunk);
        return chunks;
    }
}
