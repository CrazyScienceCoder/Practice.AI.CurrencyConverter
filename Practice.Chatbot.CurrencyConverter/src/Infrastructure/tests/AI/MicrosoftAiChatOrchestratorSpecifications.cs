using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;
using AiChatMessage = Microsoft.Extensions.AI.ChatMessage;
using DomainChatMessage = Practice.Chatbot.CurrencyConverter.Domain.Chat.ChatMessage;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.AI;

public sealed partial class MicrosoftAiChatOrchestratorSpecifications
{
    private static Conversation BuildConversation(params Action<Conversation>[] configure)
    {
        var conversation = Conversation.Start("user-123");
        foreach (var action in configure)
            action(conversation);
        return conversation;
    }

    [Fact]
    public async Task StreamReplyAsync_SingleChunk_YieldsSingleChunk()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("What is the EUR rate?"));
        var sut = new TestBuilder().WithStreamingUpdates("The EUR rate is 1.08.").Build();

        var chunks = await CollectChunksAsync(sut, conversation, TestContext.Current.CancellationToken);

        chunks.Should().ContainSingle("The EUR rate is 1.08.");
    }

    [Fact]
    public async Task StreamReplyAsync_MultipleChunks_YieldsAllChunksInOrder()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("Convert USD to EUR."));
        var sut = new TestBuilder().WithStreamingUpdates("The ", "rate ", "is 0.92.").Build();

        var chunks = await CollectChunksAsync(sut, conversation, TestContext.Current.CancellationToken);

        chunks.Should().ContainInOrder("The ", "rate ", "is 0.92.");
    }

    [Fact]
    public async Task StreamReplyAsync_EmptyStream_YieldsNoChunks()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("Hello?"));
        var sut = new TestBuilder().WithEmptyStream().Build();

        var chunks = await CollectChunksAsync(sut, conversation, TestContext.Current.CancellationToken);

        chunks.Should().BeEmpty();
    }

    [Fact]
    public async Task StreamReplyAsync_UpdateWithNullOrEmptyText_SkipsUpdate()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("Hi"));

        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Returns(UpdatesWithNullAndEmptyText(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        var chunks = await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        chunks.Should().ContainSingle("hello");

        static async IAsyncEnumerable<ChatResponseUpdate> UpdatesWithNullAndEmptyText(
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            yield return new ChatResponseUpdate((ChatRole?)null, (string?)null);  // null text
            yield return new ChatResponseUpdate((ChatRole?)null, "");             // empty text
            yield return new ChatResponseUpdate((ChatRole?)null, "hello");
        }
    }

    [Fact]
    public async Task StreamReplyAsync_WithUserMessage_PassesUserRoleToClient()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("What is USD?"));

        IEnumerable<AiChatMessage>? capturedMessages = null;
        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AiChatMessage>, ChatOptions?, CancellationToken>(
                (msgs, _, _) => capturedMessages = msgs)
            .Returns(EmptyStream(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        capturedMessages.Should().NotBeNull();
        capturedMessages!.Should().Contain(m => m.Role == ChatRole.User && m.Text == "What is USD?");
    }

    [Fact]
    public async Task StreamReplyAsync_WithAssistantMessage_PassesAssistantRoleToClient()
    {
        var conversation = Conversation.Reconstitute(
            ConversationId.New(),
            "user-123",
            DateTimeOffset.UtcNow,
            [DomainChatMessage.AssistantMessage("I can help with that.")]);

        IEnumerable<AiChatMessage>? capturedMessages = null;
        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AiChatMessage>, ChatOptions?, CancellationToken>(
                (msgs, _, _) => capturedMessages = msgs)
            .Returns(EmptyStream(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        capturedMessages.Should().Contain(
            m => m.Role == ChatRole.Assistant && m.Text == "I can help with that.");
    }

    [Fact]
    public async Task StreamReplyAsync_WithSystemRoleMessage_MapsToUserRoleInClient()
    {
        // The _ => ChatRole.User default branch covers MessageRole.System in BuildMessages
        var conversation = Conversation.Reconstitute(
            ConversationId.New(),
            "user-123",
            DateTimeOffset.UtcNow,
            [DomainChatMessage.SystemMessage("Override prompt.")]);

        IEnumerable<AiChatMessage>? capturedMessages = null;
        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AiChatMessage>, ChatOptions?, CancellationToken>(
                (msgs, _, _) => capturedMessages = msgs)
            .Returns(EmptyStream(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        // MessageRole.System hits the _ => ChatRole.User default arm
        capturedMessages.Should().Contain(
            m => m.Role == ChatRole.User && m.Text == "Override prompt.");
    }

    [Fact]
    public async Task StreamReplyAsync_Always_FirstMessageIsSystemPrompt()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("Hello"));

        IEnumerable<AiChatMessage>? capturedMessages = null;
        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AiChatMessage>, ChatOptions?, CancellationToken>(
                (msgs, _, _) => capturedMessages = msgs)
            .Returns(EmptyStream(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        capturedMessages.Should().NotBeNull();
        capturedMessages!.First().Role.Should().Be(ChatRole.System);
    }

    [Fact]
    public async Task StreamReplyAsync_ConversationWithNoMessages_PassesOnlySystemPrompt()
    {
        var conversation = Conversation.Start("user-123");

        IEnumerable<AiChatMessage>? capturedMessages = null;
        var chatClientMock = new Mock<IChatClient>();
        chatClientMock
            .Setup(c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IEnumerable<AiChatMessage>, ChatOptions?, CancellationToken>(
                (msgs, _, _) => capturedMessages = msgs)
            .Returns(EmptyStream(TestContext.Current.CancellationToken));

        var orchestrator = new Infrastructure.AI.MicrosoftAiChatOrchestrator(
            chatClientMock.Object,
            [],
            new Mock<Microsoft.Extensions.Logging.ILogger<Infrastructure.AI.MicrosoftAiChatOrchestrator>>().Object);

        await CollectChunksAsync(orchestrator, conversation, TestContext.Current.CancellationToken);

        capturedMessages!.Should().HaveCount(1);
        capturedMessages.First().Role.Should().Be(ChatRole.System);
    }

    [Fact]
    public async Task StreamReplyAsync_Always_CallsChatClientGetStreamingResponseAsyncOnce()
    {
        var conversation = BuildConversation(c => c.AddUserMessage("EUR rate?"));
        var builder = new TestBuilder().WithStreamingUpdates("1.08");
        var sut = builder.Build();

        await CollectChunksAsync(sut, conversation, TestContext.Current.CancellationToken);

        builder.ChatClientMock.Verify(
            c => c.GetStreamingResponseAsync(
                It.IsAny<IEnumerable<AiChatMessage>>(),
                It.IsAny<ChatOptions?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static async IAsyncEnumerable<ChatResponseUpdate> EmptyStream(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        yield break;
    }
}
