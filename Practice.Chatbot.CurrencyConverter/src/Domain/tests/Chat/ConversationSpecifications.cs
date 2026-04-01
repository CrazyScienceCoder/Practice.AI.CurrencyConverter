using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Domain.Tests.Chat;

public sealed class ConversationSpecifications
{
    [Fact]
    public void Start_ValidUserId_AssignsNonEmptyConversationId()
    {
        var conversation = Conversation.Start("user-123");

        conversation.Id.Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Start_ValidUserId_SetsUserId()
    {
        const string userId = "user-123";

        var conversation = Conversation.Start(userId);

        conversation.UserId.Should().Be(userId);
    }

    [Fact]
    public void Start_ValidUserId_SetsCreatedAtCloseToUtcNow()
    {
        var before = DateTimeOffset.UtcNow;

        var conversation = Conversation.Start("user-123");

        conversation.CreatedAt.Should().BeOnOrAfter(before)
            .And.BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Start_ValidUserId_InitializesWithEmptyMessagesList()
    {
        var conversation = Conversation.Start("user-123");

        conversation.Messages.Should().BeEmpty();
    }

    [Fact]
    public void Start_ValidUserId_SetsLastActivityAtToCreatedAt()
    {
        var conversation = Conversation.Start("user-123");

        conversation.LastActivityAt.Should().Be(conversation.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Start_EmptyOrWhiteSpaceUserId_ThrowsArgumentException(string userId)
    {
        var act = () => Conversation.Start(userId);

        act.Should().ThrowExactly<ArgumentException>()
            .Which.ParamName.Should().Be("userId");
    }

    [Fact]
    public void Start_EmptyUserId_ThrowsWithDescriptiveMessage()
    {
        var act = () => Conversation.Start(string.Empty);

        act.Should().ThrowExactly<ArgumentException>()
            .Which.Message.Should().Contain("UserId cannot be empty.");
    }

    [Fact]
    public void StartWithId_ValidParameters_UsesProvidedConversationId()
    {
        var id = ConversationId.New();

        var conversation = Conversation.StartWithId(id, "user-123");

        conversation.Id.Should().Be(id);
    }

    [Fact]
    public void StartWithId_ValidParameters_SetsUserId()
    {
        const string userId = "user-abc";

        var conversation = Conversation.StartWithId(ConversationId.New(), userId);

        conversation.UserId.Should().Be(userId);
    }

    [Fact]
    public void StartWithId_ValidParameters_InitializesWithEmptyMessagesList()
    {
        var conversation = Conversation.StartWithId(ConversationId.New(), "user-123");

        conversation.Messages.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void StartWithId_EmptyOrWhiteSpaceUserId_ThrowsArgumentException(string userId)
    {
        var act = () => Conversation.StartWithId(ConversationId.New(), userId);

        act.Should().ThrowExactly<ArgumentException>()
            .Which.ParamName.Should().Be("userId");
    }

    [Fact]
    public void StartWithId_EmptyUserId_ThrowsWithDescriptiveMessage()
    {
        var act = () => Conversation.StartWithId(ConversationId.New(), string.Empty);

        act.Should().ThrowExactly<ArgumentException>()
            .Which.Message.Should().Contain("UserId cannot be empty.");
    }

    [Fact]
    public void Reconstitute_WithMessages_LoadsAllMessages()
    {
        var id = ConversationId.New();
        var messages = new[]
        {
            ChatMessage.UserMessage("Hello"),
            ChatMessage.AssistantMessage("Hi there!")
        };

        var conversation = Conversation.Reconstitute(id, "user-123", DateTimeOffset.UtcNow, messages);

        conversation.Messages.Should().HaveCount(2);
    }

    [Fact]
    public void Reconstitute_WithMessages_SetsLastActivityAtToMaxMessageTimestamp()
    {
        var id = ConversationId.New();
        var messages = new[]
        {
            ChatMessage.UserMessage("First message"),
            ChatMessage.AssistantMessage("Second message")
        };
        var expectedLastActivity = messages.Max(m => m.Timestamp);

        var conversation = Conversation.Reconstitute(id, "user-123", DateTimeOffset.UtcNow.AddHours(-1), messages);

        conversation.LastActivityAt.Should().Be(expectedLastActivity);
    }

    [Fact]
    public void Reconstitute_WithNoMessages_SetsLastActivityAtToCreatedAt()
    {
        var id = ConversationId.New();
        var createdAt = DateTimeOffset.UtcNow.AddDays(-1);

        var conversation = Conversation.Reconstitute(id, "user-123", createdAt, []);

        conversation.LastActivityAt.Should().Be(createdAt);
    }

    [Fact]
    public void Reconstitute_WithMessages_UsesProvidedId()
    {
        var id = ConversationId.New();

        var conversation = Conversation.Reconstitute(id, "user-456", DateTimeOffset.UtcNow, []);

        conversation.Id.Should().Be(id);
    }

    [Fact]
    public void Reconstitute_WithMessages_UsesProvidedUserId()
    {
        const string userId = "user-789";

        var conversation = Conversation.Reconstitute(ConversationId.New(), userId, DateTimeOffset.UtcNow, []);

        conversation.UserId.Should().Be(userId);
    }

    [Fact]
    public void Reconstitute_WithMessages_UsesProvidedCreatedAt()
    {
        var createdAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var conversation = Conversation.Reconstitute(ConversationId.New(), "user-123", createdAt, []);

        conversation.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void AddUserMessage_ValidContent_AddsMessageToMessages()
    {
        var conversation = Conversation.Start("user-123");

        conversation.AddUserMessage("Hello");

        conversation.Messages.Should().ContainSingle();
    }

    [Fact]
    public void AddUserMessage_ValidContent_MessageHasUserRole()
    {
        var conversation = Conversation.Start("user-123");

        conversation.AddUserMessage("Hello");

        conversation.Messages[0].Role.Should().Be(MessageRole.User);
    }

    [Fact]
    public void AddUserMessage_ValidContent_MessageStoresContent()
    {
        const string content = "What is the GBP rate?";
        var conversation = Conversation.Start("user-123");

        conversation.AddUserMessage(content);

        conversation.Messages[0].Content.Should().Be(content);
    }

    [Fact]
    public void AddUserMessage_ValidContent_UpdatesLastActivityAt()
    {
        var conversation = Conversation.Start("user-123");
        var activityBefore = conversation.LastActivityAt;

        conversation.AddUserMessage("Hello");

        conversation.LastActivityAt.Should().BeOnOrAfter(activityBefore);
    }

    [Fact]
    public void AddAssistantMessage_ValidContent_AddsMessageToMessages()
    {
        var conversation = Conversation.Start("user-123");

        conversation.AddAssistantMessage("Here is the rate.");

        conversation.Messages.Should().ContainSingle();
    }

    [Fact]
    public void AddAssistantMessage_ValidContent_MessageHasAssistantRole()
    {
        var conversation = Conversation.Start("user-123");

        conversation.AddAssistantMessage("Here is the rate.");

        conversation.Messages[0].Role.Should().Be(MessageRole.Assistant);
    }

    [Fact]
    public void AddAssistantMessage_ValidContent_MessageStoresContent()
    {
        const string content = "The GBP rate is 0.78.";
        var conversation = Conversation.Start("user-123");

        conversation.AddAssistantMessage(content);

        conversation.Messages[0].Content.Should().Be(content);
    }

    [Fact]
    public void AddAssistantMessage_ValidContent_UpdatesLastActivityAt()
    {
        var conversation = Conversation.Start("user-123");
        var activityBefore = conversation.LastActivityAt;

        conversation.AddAssistantMessage("Response");

        conversation.LastActivityAt.Should().BeOnOrAfter(activityBefore);
    }

    [Fact]
    public void Messages_Always_ReturnsReadOnlyList()
    {
        var conversation = Conversation.Start("user-123");

        var messages = conversation.Messages;

        messages.Should().BeAssignableTo<IReadOnlyList<ChatMessage>>();
    }

    [Fact]
    public void AddUserMessage_ThenAddAssistantMessage_MessagesAreInInsertionOrder()
    {
        var conversation = Conversation.Start("user-123");

        conversation.AddUserMessage("User question");
        conversation.AddAssistantMessage("Assistant answer");

        conversation.Messages.Should().HaveCount(2);
        conversation.Messages[0].Role.Should().Be(MessageRole.User);
        conversation.Messages[1].Role.Should().Be(MessageRole.Assistant);
    }
}
