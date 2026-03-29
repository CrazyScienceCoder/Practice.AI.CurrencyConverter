namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Chat.GetHistory;

public partial class GetChatHistoryQueryHandlerSpecifications
{
    [Fact]
    public async Task Handle_ConversationFoundAndUserMatches_ReturnsResponseWithMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(testBuilder.DefaultQuery.ConversationId);
        result.Messages.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsUserMessageRoleToLowerCase()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Messages[0].Role.Should().Be("user");
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsAssistantMessageRoleToLowerCase()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Messages[1].Role.Should().Be("assistant");
    }

    [Fact]
    public async Task Handle_ConversationFound_MapsMessageContentAndTimestamp()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Messages[0].Content.Should().Be("What is the exchange rate for USD?");
        result.Messages[0].Timestamp.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Handle_ConversationFoundWithNoMessages_ReturnsEmptyMessagesList()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationFoundWithNoMessages();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Should().NotBeNull();
        result.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ConversationNotFound_ReturnsEmptyMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationNotFound();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(testBuilder.DefaultQuery.ConversationId);
        result.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ConversationBelongsToDifferentUser_ReturnsEmptyMessages()
    {
        var testBuilder = new TestBuilder()
            .SetupConversationBelongsToDifferentUser();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.RepositoryMock.Verify();
        result.Should().NotBeNull();
        result.ConversationId.Should().Be(testBuilder.DefaultQuery.ConversationId);
        result.Messages.Should().BeEmpty();
    }
}
