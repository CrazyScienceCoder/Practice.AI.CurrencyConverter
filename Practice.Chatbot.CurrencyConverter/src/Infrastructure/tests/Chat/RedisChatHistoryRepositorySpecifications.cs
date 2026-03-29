using Microsoft.Extensions.Caching.Distributed;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Chat;

public sealed partial class RedisChatHistoryRepositorySpecifications
{
    private static readonly Guid ConversationGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private const string UserId = "user-abc";
    private static readonly DateTimeOffset CreatedAt = new(2024, 1, 15, 10, 0, 0, TimeSpan.Zero);

    // ─── FindAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task FindAsync_CacheMiss_ReturnsNull()
    {
        var sut = new TestBuilder().WithCacheMiss().Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task FindAsync_CacheMiss_CallsGetAsyncOnce()
    {
        var builder = new TestBuilder().WithCacheMiss();
        var sut = builder.Build();

        await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task FindAsync_CacheHitWithUserMessage_ReturnsConversationWithCorrectId()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.User, "Hello", CreatedAt.AddMinutes(1))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result!.Id.ToString().Should().Be(ConversationGuid.ToString());
    }

    [Fact]
    public async Task FindAsync_CacheHitWithUserMessage_ReturnsConversationWithCorrectUserId()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.User, "Hello", CreatedAt.AddMinutes(1))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result!.UserId.Should().Be(UserId);
    }

    [Fact]
    public async Task FindAsync_CacheHitWithUserMessage_MapsMessageRoleCorrectly()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.User, "Hello", CreatedAt.AddMinutes(1))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result!.Messages.Should().HaveCount(1);
        result.Messages[0].Role.Should().Be(MessageRole.User);
        result.Messages[0].Content.Should().Be("Hello");
    }

    [Fact]
    public async Task FindAsync_CacheHitWithAssistantMessage_MapsAssistantRoleCorrectly()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.Assistant, "The rate is 1.08.", CreatedAt.AddMinutes(2))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result!.Messages[0].Role.Should().Be(MessageRole.Assistant);
        result.Messages[0].Content.Should().Be("The rate is 1.08.");
    }

    [Fact]
    public async Task FindAsync_CacheHitWithSystemMessage_MapsToSystemMessageCorrectly()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.System, "You are an assistant.", CreatedAt.AddMinutes(1))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result!.Messages.Should().HaveCount(1);
        result.Messages[0].Role.Should().Be(MessageRole.System);
    }

    [Fact]
    public async Task FindAsync_CacheHitWithMultipleMessages_ReturnsAllMessagesInOrder()
    {
        var messages = new[]
        {
            new TestMessageDto(MessageRole.User, "Question?", CreatedAt.AddMinutes(1)),
            new TestMessageDto(MessageRole.Assistant, "Answer.", CreatedAt.AddMinutes(2))
        };
        var builder = new TestBuilder().WithCachedConversation(ConversationGuid, UserId, CreatedAt, messages);
        var sut = builder.Build();

        var result = await sut.FindAsync(
            ConversationId.From(ConversationGuid.ToString()),
            TestContext.Current.CancellationToken);

        result!.Messages.Should().HaveCount(2);
        result.Messages[0].Content.Should().Be("Question?");
        result.Messages[1].Content.Should().Be("Answer.");
    }

    // ─── SaveAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task SaveAsync_Always_CallsSetAsyncOnCacheOnce()
    {
        var conversation = Conversation.Start(UserId);
        conversation.AddUserMessage("Hello");

        var builder = new TestBuilder();
        var sut = builder.Build();

        await sut.SaveAsync(conversation, TestContext.Current.CancellationToken);

        builder.CacheMock.Verify(
            c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_Always_CacheKeyContainsConversationId()
    {
        var conversation = Conversation.StartWithId(
            ConversationId.From(ConversationGuid.ToString()), UserId);

        string? capturedKey = null;
        var builder = new TestBuilder();
        builder.CacheMock
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (key, _, _, _) => capturedKey = key)
            .Returns(Task.CompletedTask);
        var sut = builder.Build();

        await sut.SaveAsync(conversation, TestContext.Current.CancellationToken);

        capturedKey.Should().Contain(ConversationGuid.ToString());
    }

    [Fact]
    public async Task SaveAsync_Always_CacheEntryOptionsHasTtlSet()
    {
        var conversation = Conversation.Start(UserId);
        conversation.AddUserMessage("test");

        DistributedCacheEntryOptions? capturedOptions = null;
        var builder = new TestBuilder();
        builder.CacheMock
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, _, opts, _) => capturedOptions = opts)
            .Returns(Task.CompletedTask);
        var sut = builder.Build();

        await sut.SaveAsync(conversation, TestContext.Current.CancellationToken);

        capturedOptions.Should().NotBeNull();
        capturedOptions!.AbsoluteExpirationRelativeToNow.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task SaveAsync_Always_SerializesConversationBytesToCache()
    {
        var conversation = Conversation.Start(UserId);
        conversation.AddUserMessage("Hello");
        conversation.AddAssistantMessage("Hi there!");

        byte[]? capturedBytes = null;
        var builder = new TestBuilder();
        builder.CacheMock
            .Setup(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (_, bytes, _, _) => capturedBytes = bytes)
            .Returns(Task.CompletedTask);
        var sut = builder.Build();

        await sut.SaveAsync(conversation, TestContext.Current.CancellationToken);

        capturedBytes.Should().NotBeNullOrEmpty();
        var json = System.Text.Encoding.UTF8.GetString(capturedBytes!);
        json.Should().Contain(UserId);
    }
}
